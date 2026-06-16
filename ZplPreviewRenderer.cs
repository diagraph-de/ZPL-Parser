#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZintSupport = Diagraph.Labelparser.ZPL.ZintSupport;

namespace Diagraph.Labelparser.ZPL;

public sealed class ZplPreviewRenderer
{
    private const int DefaultCanvasWidth = 812;
    private const int DefaultCanvasHeight = 1218;
    private const int Margin = 24;

    public PreviewRenderResult Render(string zpl)
    {
        return Render(zpl, PreviewSurfaceSettings.Default);
    }

    public PreviewRenderResult Render(string zpl, PreviewSurfaceSettings? settings)
    {
        var result = new PreviewRenderResult();
        var input = zpl ?? string.Empty;
        var parser = new ZplParser(Encoding.UTF8.GetBytes(input));
        var elements = parser.Elements ?? new List<BaseElement>();
        var warnings = new List<string>();
        var previewSettings = settings ?? PreviewSurfaceSettings.Default;

        result.Tree = parser.GetTree();
        result.Elements.AddRange(elements.Select(CreateElementInfo));

        var renderOptions = new ZPLRenderOptions
        {
            DisplayComments = true,
            AddEmptyLineBeforeElementStart = false,
            CompressedRendering = false
        };

        try
        {
            result.NormalizedZpl = new ZPLEngine(elements).ToZPLString(renderOptions);
        }
        catch (Exception ex)
        {
            result.NormalizedZpl = input;
            result.Error = ex.Message;
        }

        try
        {
            result.PreviewBitmap = RenderBitmap(elements, parser.Error, warnings, previewSettings, out var localWidth,
                out var localHeight);
            result.CanvasWidth = localWidth;
            result.CanvasHeight = localHeight;
            if (!string.IsNullOrWhiteSpace(parser.Error))
                warnings.Add(parser.Error);
            if (warnings.Count > 0)
                result.Error = string.Join(Environment.NewLine, warnings.Distinct());
        }
        catch (Exception ex)
        {
            result.Error = string.IsNullOrWhiteSpace(result.Error)
                ? ex.Message
                : result.Error + Environment.NewLine + ex.Message;
            result.PreviewBitmap = CreateFallbackBitmap(result.Error);
            result.CanvasWidth = result.PreviewBitmap.Width;
            result.CanvasHeight = result.PreviewBitmap.Height;
        }

        return result;
    }

    private static PreviewElementInfo CreateElementInfo(BaseElement element)
    {
        var summary = element.RenderToString();
        if (summary.Length > 120)
            summary = summary.Substring(0, 117) + "...";

        return new PreviewElementInfo
        {
            Type = element.GetType().Name,
            Id = element.Id,
            Summary = summary
        };
    }

    private static Bitmap CreateFallbackBitmap(string message)
    {
        var bitmap = new Bitmap(DefaultCanvasWidth, DefaultCanvasHeight);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.WhiteSmoke);
        using var pen = new Pen(Color.DarkRed, 2);
        graphics.DrawRectangle(pen, 12, 12, bitmap.Width - 24, bitmap.Height - 24);
        using var brush = new SolidBrush(Color.DarkRed);
        using var font = new Font("Segoe UI", 11, FontStyle.Bold, GraphicsUnit.Point);
        graphics.DrawString(message, font, brush, new RectangleF(24, 24, bitmap.Width - 48, bitmap.Height - 48));
        return bitmap;
    }

    private static Bitmap RenderBitmap(IReadOnlyList<BaseElement> elements, string parserError, List<string> warnings,
        PreviewSurfaceSettings settings, out int width, out int height)
    {
        var labelWidth = elements.OfType<PrintWidth>().LastOrDefault()?.Width ?? 0;
        var labelHeight = elements.OfType<LabelLength>().LastOrDefault()?.Length ?? 0;
        var home = elements.OfType<LabelHome>().LastOrDefault();
        var top = elements.OfType<LabelTop>().LastOrDefault();
        var shift = elements.OfType<LabelShfit>().LastOrDefault();
        var reverse = elements.OfType<LabelReverse>().LastOrDefault();

        var offsetX = (home?.PositionX ?? 0) + (shift?.ShiftLeft ?? 0);
        var offsetY = (home?.PositionY ?? 0) + (top?.Top ?? 0);
        var reverseColors = reverse?.Reverse == Enums.YesNo.Y;

        var storedGraphics = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        var estimatedBounds = new List<Rectangle>();

        foreach (var element in elements)
        {
            if (element is DownloadGraphic downloadGraphic && downloadGraphic.Image != null)
                storedGraphics[GraphicKey(downloadGraphic.DestinationDevice, downloadGraphic.ImageName,
                    downloadGraphic.FileNameExtension)] = downloadGraphic.Image;

            estimatedBounds.Add(EstimateBounds(element, offsetX, offsetY));
        }

        width = labelWidth > 0 ? labelWidth : GetFallbackCanvasWidth(settings);
        height = labelHeight > 0 ? labelHeight : GetFallbackCanvasHeight(settings);

        var bitmap = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(reverseColors ? Color.Black : Color.White);

            var renderState = new RenderState
            {
                OffsetX = offsetX,
                OffsetY = offsetY,
                Foreground = reverseColors ? Color.White : Color.Black,
                Background = reverseColors ? Color.Black : Color.White
            };

            foreach (var element in elements)
                try
                {
                    DrawElement(graphics, element, renderState, storedGraphics, warnings);
                }
                catch (Exception ex)
                {
                    warnings.Add($"{element.GetType().Name}: {ex.Message}");
                }
        }

        return bitmap;
    }

    private static int GetFallbackCanvasWidth(PreviewSurfaceSettings settings)
    {
        if (settings.LabelWidthInches > 0 && settings.PrintDensityDpmm > 0)
        {
            var dpi = GetEffectiveDpi(settings.PrintDensityDpmm);
            return Math.Max(DefaultCanvasWidth, (int)Math.Round(settings.LabelWidthInches * dpi));
        }

        return DefaultCanvasWidth;
    }

    private static int GetFallbackCanvasHeight(PreviewSurfaceSettings settings)
    {
        if (settings.LabelHeightInches > 0 && settings.PrintDensityDpmm > 0)
        {
            var dpi = GetEffectiveDpi(settings.PrintDensityDpmm);
            return Math.Max(DefaultCanvasHeight, (int)Math.Round(settings.LabelHeightInches * dpi));
        }

        return DefaultCanvasHeight;
    }

    private static int GetEffectiveDpi(int densityDpmm)
    {
        return Math.Max(1, (int)Math.Floor(densityDpmm * 25.4));
    }

    private static Rectangle EstimateBounds(BaseElement element, int offsetX, int offsetY)
    {
        switch (element)
        {
            case GraphicDiagonalLine graphicDiagonalLine:
                return Rect(graphicDiagonalLine.Origin, offsetX, offsetY, graphicDiagonalLine.Width,
                    graphicDiagonalLine.Height);
            case GraphicEllipse graphicEllipse:
                return Rect(graphicEllipse.Origin, offsetX, offsetY, graphicEllipse.Width, graphicEllipse.Height);
            case GraphicCircle graphicCircle:
                return Rect(graphicCircle.Origin, offsetX, offsetY, graphicCircle.Diameter, graphicCircle.Diameter);
            case GraphicBox graphicBox:
                return Rect(graphicBox.Origin, offsetX, offsetY,
                    graphicBox.Width, graphicBox.Height);
            case GraphicSymbol graphicSymbol:
                return Rect(graphicSymbol.Origin, offsetX, offsetY, graphicSymbol.Width, graphicSymbol.Height);
            case BarcodeQR barcodeQR:
                return Rect(barcodeQR.Origin, offsetX, offsetY, 140 + barcodeQR.MagnificationFactor * 6,
                    140 + barcodeQR.MagnificationFactor * 6);
            case BarcodeDatamatrix dataMatrix:
                return Rect(dataMatrix.Origin, offsetX, offsetY, 140, 140);
            case BarcodeCode39 barcodeCode39:
                return Rect(barcodeCode39.Origin, offsetX, offsetY,
                    EstimateBarcodeWidth(ResolveBarcodeContent(barcodeCode39), 13, 20), barcodeCode39.Height + 42);
            case BarcodeCode128 barcodeCode128:
                return Rect(barcodeCode128.Origin, offsetX, offsetY,
                    EstimateBarcodeWidth(ResolveBarcodeContent(barcodeCode128), 14, 40), barcodeCode128.Height + 42);
            case BarcodeAnsiCodabar barcodeAnsiCodabar:
                return Rect(barcodeAnsiCodabar.Origin, offsetX, offsetY,
                    EstimateBarcodeWidth(ResolveBarcodeContent(barcodeAnsiCodabar), 13, 20),
                    barcodeAnsiCodabar.Height + 42);
            case GraphicField graphicField:
                return graphicField.Bitmap == null
                    ? Rect(graphicField.Origin, offsetX, offsetY, 120, 80)
                    : Rect(graphicField.Origin, offsetX, offsetY, graphicField.Bitmap.Width,
                        graphicField.Bitmap.Height);
            case RecallGraphic recallGraphic:
                return Rect(recallGraphic.Origin, offsetX, offsetY, 120, 80);
            case DownloadGraphic downloadGraphic:
                return downloadGraphic.Image == null
                    ? new Rectangle(offsetX + 20, offsetY + 20, 120, 80)
                    : new Rectangle(offsetX + 20, offsetY + 20, downloadGraphic.Image.Width,
                        downloadGraphic.Image.Height);
            case SingleLineFieldBlock singleLineFieldBlock:
                return Rect(singleLineFieldBlock.Origin, offsetX, offsetY, singleLineFieldBlock.Width,
                    Math.Max(singleLineFieldBlock.Font?.FontHeight ?? 24, 32));
            case FieldBlock fieldBlock:
                return Rect(fieldBlock.Origin, offsetX, offsetY, fieldBlock.Width,
                    Math.Max(fieldBlock.Font?.FontHeight ?? 24, 32) * Math.Max(1, fieldBlock.MaxNumberOfLines));
            case TextBlock textBlock:
                return Rect(textBlock.Origin, offsetX, offsetY, textBlock.Width, textBlock.Height);
            case TextField textField:
                return Rect(textField.Origin, offsetX, offsetY,
                    EstimateTextWidth(textField.Text, textField.Font),
                    Math.Max(textField.Font?.FontHeight ?? 24, 32));
            default:
                return new Rectangle(offsetX + 10, offsetY + 10, 10, 10);
        }
    }

    private static Rectangle Rect(FieldOrigin origin, int offsetX, int offsetY, int width, int height)
    {
        var x = (origin?.PositionX ?? 0) + offsetX;
        var y = (origin?.PositionY ?? 0) + offsetY;
        return new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
    }

    private static void DrawElement(Graphics graphics, BaseElement element, RenderState state,
        IReadOnlyDictionary<string, Image> storedGraphics, List<string> warnings)
    {
        switch (element)
        {
            case GraphicDiagonalLine graphicDiagonalLine:
                DrawDiagonal(graphics, graphicDiagonalLine, state);
                break;
            case GraphicEllipse graphicEllipse:
                DrawEllipse(graphics, ResolveOrigin(graphicEllipse.Origin, state), state, graphicEllipse.Width,
                    graphicEllipse.Height,
                    graphicEllipse.BorderThickness);
                break;
            case GraphicCircle graphicCircle:
                DrawEllipse(graphics, ResolveOrigin(graphicCircle.Origin, state), state, graphicCircle.Diameter,
                    graphicCircle.Diameter,
                    graphicCircle.BorderThickness);
                break;
            case GraphicBox graphicBox:
                DrawBox(graphics, ResolveOrigin(graphicBox.Origin, state), state, graphicBox.Width, graphicBox.Height,
                    graphicBox.BorderThickness,
                    graphicBox.LineColor);
                break;
            case GraphicSymbol graphicSymbol:
                DrawSymbol(graphics, graphicSymbol, state);
                break;
            case SingleLineFieldBlock singleLineFieldBlock:
                DrawTextBlock(graphics, singleLineFieldBlock, state, true);
                break;
            case FieldBlock fieldBlock:
                DrawTextBlock(graphics, fieldBlock, state);
                break;
            case TextBlock textBlock:
                DrawTextBlock(graphics, textBlock, state);
                break;
            case BarcodeCode39 barcodeCode39:
                DrawCode39(graphics, barcodeCode39, state, warnings);
                break;
            case BarcodeCode128 barcodeCode128:
                DrawCode128(graphics, barcodeCode128, state, warnings);
                break;
            case BarcodeAnsiCodabar barcodeAnsiCodabar:
                DrawCodabar(graphics, barcodeAnsiCodabar, state, warnings);
                break;
            case BarcodeQR barcodeQR:
            {
                var content = ResolveBarcodeContent(barcodeQR);
                if (string.IsNullOrWhiteSpace(content))
                {
                    state.PendingQrBarcode = barcodeQR;
                    break;
                }

                DrawQrCode(graphics, barcodeQR, content, state, warnings);
                break;
            }
            case BarcodeDatamatrix barcodeDatamatrix:
            {
                var content = ResolveBarcodeContent(barcodeDatamatrix);
                if (string.IsNullOrWhiteSpace(content))
                {
                    state.PendingDataMatrixBarcode = barcodeDatamatrix;
                    break;
                }

                DrawDataMatrix(graphics, barcodeDatamatrix, content, state, warnings);
                break;
            }
            case GraphicField graphicField:
                DrawImage(graphics, ResolveOrigin(graphicField.Origin, state), state, graphicField.Bitmap,
                    graphicField.Bitmap?.Width ?? 120, graphicField.Bitmap?.Height ?? 80);
                break;
            case DownloadGraphic downloadGraphic:
                break;
            case RecallGraphic recallGraphic:
            {
                var key = GraphicKey(recallGraphic.StorageDevice, recallGraphic.ImageName, recallGraphic.Extension);
                storedGraphics.TryGetValue(key, out var image);
                var width = image?.Width ?? 120;
                var height = image?.Height ?? 80;
                if (recallGraphic.MagnificationFactorX > 0)
                    width *= recallGraphic.MagnificationFactorX;
                if (recallGraphic.MagnificationFactorY > 0)
                    height *= recallGraphic.MagnificationFactorY;
                DrawImage(graphics, ResolveOrigin(recallGraphic.Origin, state), state, image, width, height);
                break;
            }
            case FieldData fieldData:
            {
                if (state.PendingQrBarcode != null)
                {
                    var content = ExtractStructuredBarcodeContent(fieldData.Data);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var pendingQrBarcode = state.PendingQrBarcode;
                        state.PendingQrBarcode = null;
                        state.SuppressNextTextField = true;
                        DrawQrCode(graphics, pendingQrBarcode, content, state, warnings);
                    }

                    break;
                }

                if (state.PendingDataMatrixBarcode != null)
                {
                    var content = ExtractStructuredBarcodeContent(fieldData.Data);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var pendingDataMatrixBarcode = state.PendingDataMatrixBarcode;
                        state.PendingDataMatrixBarcode = null;
                        state.SuppressNextTextField = true;
                        DrawDataMatrix(graphics, pendingDataMatrixBarcode, content, state, warnings);
                    }

                    break;
                }

                break;
            }
            case FieldOrigin fieldOrigin:
                state.PendingFieldOrigin = fieldOrigin;
                break;
            case BarcodeFieldDefault barcodeFieldDefault:
                state.BarcodeDefaults = barcodeFieldDefault;
                break;
            case FieldReversePrint:
                state.ReverseField = true;
                break;
            case FieldSeparator:
                state.ReverseField = false;
                state.PendingFieldOrigin = null;
                state.PendingBarcodeText = null;
                state.PendingBarcodeOrigin = null;
                break;
            case TextField textField:
                if (state.SuppressNextTextField)
                {
                    state.SuppressNextTextField = false;
                    break;
                }
                DrawTextField(graphics, textField, state, state.ReverseField || textField.ReversePrint);
                break;
        }
    }

    private static void DrawTextField(Graphics graphics, TextField textField, RenderState state, bool reverse,
        Rectangle? overrideBounds = null)
    {
        var origin = ResolveOrigin(textField.Origin, state);
        var x = state.OffsetX + (origin?.PositionX ?? 0);
        var y = state.OffsetY + (origin?.PositionY ?? 0);
        using var font = CreateFont(textField.Font, textField.Font?.FontName != "A");
        var measuredRect = MeasureTextRect(graphics, textField.Text, font, x, y);
        var rect = overrideBounds ?? measuredRect;
        var isBarcodeLabel = TryGetBarcodeTextOverrideBounds(textField, state, measuredRect, out var barcodeRect);
        if (isBarcodeLabel)
            return;
        if ((textField.Text?.Length ?? 0) <= 2 && (textField.Font?.FontHeight ?? 0) >= 100)
            rect = new Rectangle(
                rect.Left,
                Math.Max(0, rect.Top - Math.Max(18, (textField.Font?.FontHeight ?? 0) / 4)),
                Math.Max(rect.Width, Math.Max(200, (textField.Font?.FontHeight ?? 0) + 24)),
                Math.Max(rect.Height, Math.Max(220, (textField.Font?.FontHeight ?? 0) + 46)));
        else
            rect = new Rectangle(rect.Left, rect.Top,
                Math.Max(rect.Width, Math.Max(2000, EstimateTextWidth(textField.Text, textField.Font) + 20)),
                rect.Height);

        using var foreBrush = new SolidBrush(reverse ? state.Background : state.Foreground);
        if (reverse)
        {
            using var backBrush = new SolidBrush(state.Foreground);
            graphics.FillRectangle(backBrush, rect);
        }

        using var stringFormat = new StringFormat(StringFormat.GenericTypographic)
        {
            Trimming = StringTrimming.None,
            Alignment = isBarcodeLabel ? StringAlignment.Center : StringAlignment.Near,
            LineAlignment = isBarcodeLabel ? StringAlignment.Near : StringAlignment.Near
        };

        if (isBarcodeLabel)
            stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
        else if ((textField.Text?.Length ?? 0) <= 2 && (textField.Font?.FontHeight ?? 0) >= 100)
        {
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
        }

        graphics.DrawString(textField.Text ?? string.Empty, font, foreBrush, rect, stringFormat);
    }

    private static bool TryGetBarcodeTextOverrideBounds(TextField textField, RenderState state, Rectangle measuredRect,
        out Rectangle overrideBounds)
    {
        overrideBounds = measuredRect;

        if (state.PendingBarcodeText == null || state.PendingBarcodeBounds == null)
            return false;

        var origin = ResolveOrigin(textField.Origin, state);
        var matchesText = string.Equals(textField.Text ?? string.Empty, state.PendingBarcodeText,
            StringComparison.Ordinal);
        var matchesOrigin = origin != null && state.PendingBarcodeOrigin != null &&
                            origin.PositionX == state.PendingBarcodeOrigin.PositionX &&
                            origin.PositionY == state.PendingBarcodeOrigin.PositionY;

        if (!matchesText || !matchesOrigin)
            return false;

        overrideBounds = new Rectangle(
            state.PendingBarcodeBounds.Value.Left,
            state.PendingBarcodeBounds.Value.Bottom + 4,
            state.PendingBarcodeBounds.Value.Width,
            measuredRect.Height);
        state.PendingBarcodeText = null;
        state.PendingBarcodeOrigin = null;
        state.PendingBarcodeBounds = null;
        return true;
    }

    private static void DrawTextBlock(Graphics graphics, TextField field, RenderState state, bool singleLine = false)
    {
        var origin = ResolveOrigin(field.Origin, state);
        var x = state.OffsetX + (origin?.PositionX ?? 0);
        var y = state.OffsetY + (origin?.PositionY ?? 0);

        var width = 260;
        var height = 80;
        switch (field)
        {
            case SingleLineFieldBlock singleLineFieldBlock:
                width = singleLineFieldBlock.Width;
                height = Math.Max(singleLineFieldBlock.Font?.FontHeight ?? 24, 32);
                break;
            case FieldBlock fieldBlock:
                width = fieldBlock.Width;
                height = Math.Max(fieldBlock.MaxNumberOfLines * Math.Max(fieldBlock.Font?.FontHeight ?? 24, 24), 32);
                break;
            case TextBlock textBlock:
                width = textBlock.Width;
                height = textBlock.Height;
                break;
        }

        var rect = new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
        var reverse = state.ReverseField || field.ReversePrint;
        using var backgroundBrush = new SolidBrush(reverse ? state.Foreground : state.Background);
        graphics.FillRectangle(backgroundBrush, rect);
        using var font = CreateFont(field.Font, field.Font?.FontName != "A");
        using var brush = new SolidBrush(reverse ? state.Background : state.Foreground);
        using var stringFormat = new StringFormat(StringFormat.GenericTypographic)
        {
            Trimming = StringTrimming.None
        };
        if (singleLine)
            stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
        graphics.DrawString(field.Text ?? string.Empty, font, brush, rect, stringFormat);
    }

    private static void DrawBox(Graphics graphics, FieldOrigin? origin, RenderState state, int width, int height,
        int borderThickness, Enums.BlackWhite lineColor)
    {
        var resolvedOrigin = ResolveOrigin(origin, state);
        var rect = new Rectangle(state.OffsetX + (resolvedOrigin?.PositionX ?? 0),
            state.OffsetY + (resolvedOrigin?.PositionY ?? 0),
            Math.Max(1, width), Math.Max(1, height));
        var thickness = Math.Max(1, borderThickness);
        var color = lineColor == Enums.BlackWhite.B ? state.Foreground : state.Background;
        if (state.ReverseField)
            color = color == state.Foreground ? state.Background : state.Foreground;

        if (thickness >= Math.Min(rect.Width, rect.Height))
        {
            using var brush = new SolidBrush(color);
            graphics.FillRectangle(brush, rect);
            return;
        }

        using var pen = new Pen(color, thickness);
        graphics.DrawRectangle(pen, rect);
    }

    private static void DrawEllipse(Graphics graphics, FieldOrigin? origin, RenderState state, int width, int height,
        int borderThickness)
    {
        var resolvedOrigin = ResolveOrigin(origin, state);
        var rect = new Rectangle(state.OffsetX + (resolvedOrigin?.PositionX ?? 0),
            state.OffsetY + (resolvedOrigin?.PositionY ?? 0),
            Math.Max(1, width), Math.Max(1, height));
        var color = state.ReverseField ? state.Background : state.Foreground;
        using var pen = new Pen(color, Math.Max(1, borderThickness));
        graphics.DrawEllipse(pen, rect);
    }

    private static void DrawDiagonal(Graphics graphics, GraphicDiagonalLine line, RenderState state)
    {
        var rect = new Rectangle(state.OffsetX + (line.Origin?.PositionX ?? 0),
            state.OffsetY + (line.Origin?.PositionY ?? 0),
            Math.Max(1, line.Width), Math.Max(1, line.Height));
        var color = state.ReverseField ? state.Background : state.Foreground;
        using var pen = new Pen(color, Math.Max(1, line.BorderThickness));
        graphics.DrawLine(pen,
            line.RightLeaningiagonal ? rect.Left : rect.Left, line.RightLeaningiagonal ? rect.Bottom : rect.Top,
            line.RightLeaningiagonal ? rect.Right : rect.Right, line.RightLeaningiagonal ? rect.Top : rect.Bottom);
    }

    private static void DrawSymbol(Graphics graphics, GraphicSymbol symbol, RenderState state)
    {
        var rect = new Rectangle(state.OffsetX + (symbol.Origin?.PositionX ?? 0),
            state.OffsetY + (symbol.Origin?.PositionY ?? 0),
            Math.Max(1, symbol.Width), Math.Max(1, symbol.Height));
        var color = state.ReverseField ? state.Background : state.Foreground;
        using var pen = new Pen(color, 2);
        graphics.DrawEllipse(pen, rect);
        using var font = new Font("Segoe UI Symbol", Math.Max(8, rect.Height / 2), FontStyle.Bold, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(color);
        using var stringFormat = new StringFormat(StringFormat.GenericTypographic)
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None
        };
        graphics.DrawString(symbol.Character.ToString().Substring(0, 1), font, brush, rect, stringFormat);
    }

    private static void DrawCode39(Graphics graphics, BarcodeCode39 barcode, RenderState state, List<string> warnings)
    {
        var barcodeDefaults = state.BarcodeDefaults ?? BarcodeFieldDefault.Current;
        var content = ResolveBarcodeContent(barcode);
        content = ApplyCode39Checksum(content, barcode.Mod43CheckDigit);
        if (string.IsNullOrWhiteSpace(content))
        {
            warnings.Add("Code39 barcode skipped because content is empty.");
            return;
        }

        var moduleWidth = Math.Max(1, barcodeDefaults?.ModuleWidth ?? 2);
        var barHeight = Math.Max(1, barcode.Height > 0 ? barcode.Height : barcodeDefaults?.Height ?? 10);
        var foreground = state.ReverseField ? state.Background : state.Foreground;
        var background = state.ReverseField ? state.Foreground : state.Background;
        var bitmap = CreateZintLinearBitmap(
            ZintSupport.BarcodeType.BARCODE_EXCODE39,
            content,
            moduleWidth,
            barHeight,
            barcode.PrintInterpretationLine,
            barcode.Orientation.ToString(),
            foreground,
            background);
        DrawBitmap(graphics, ResolveOrigin(barcode.Origin, state), state, bitmap);
        state.PendingBarcodeText = content;
        state.PendingBarcodeOrigin = ResolveOrigin(barcode.Origin, state);
        state.PendingBarcodeBounds = new Rectangle(
            state.OffsetX + (ResolveOrigin(barcode.Origin, state)?.PositionX ?? 0),
            state.OffsetY + (ResolveOrigin(barcode.Origin, state)?.PositionY ?? 0), bitmap.Width, bitmap.Height);
        bitmap.Dispose();
    }

    private static void DrawCode128(Graphics graphics, BarcodeCode128 barcode, RenderState state, List<string> warnings)
    {
        var barcodeDefaults = state.BarcodeDefaults ?? BarcodeFieldDefault.Current;
        var content = ResolveBarcodeContent(barcode);
        if (string.IsNullOrWhiteSpace(content))
        {
            warnings.Add("Code128 barcode skipped because content is empty.");
            return;
        }

        var moduleWidth = Math.Max(1, barcodeDefaults?.ModuleWidth ?? 2);
        var barHeight = Math.Max(1, barcode.Height > 0 ? barcode.Height : barcodeDefaults?.Height ?? 10);
        var foreground = state.ReverseField ? state.Background : state.Foreground;
        var background = state.ReverseField ? state.Foreground : state.Background;
        var isGs1 = barcode.UCCCheckDigit == Enums.YesNo.Y || IsGs1Payload(content);
        var bitmap = CreateZintLinearBitmap(
            isGs1 ? ZintSupport.BarcodeType.BARCODE_EAN128 : ZintSupport.BarcodeType.BARCODE_CODE128,
            content,
            moduleWidth,
            barHeight,
            true,
            barcode.Orientation.ToString(),
            foreground,
            background,
            isGs1);
        DrawBitmap(graphics, ResolveOrigin(barcode.Origin, state), state, bitmap);
        state.PendingBarcodeText = content;
        state.PendingBarcodeOrigin = ResolveOrigin(barcode.Origin, state);
        state.PendingBarcodeBounds = new Rectangle(
            state.OffsetX + (ResolveOrigin(barcode.Origin, state)?.PositionX ?? 0),
            state.OffsetY + (ResolveOrigin(barcode.Origin, state)?.PositionY ?? 0), bitmap.Width, bitmap.Height);
        bitmap.Dispose();
    }

    private static void DrawCodabar(Graphics graphics, BarcodeAnsiCodabar barcode, RenderState state,
        List<string> warnings)
    {
        var barcodeDefaults = state.BarcodeDefaults ?? BarcodeFieldDefault.Current;
        var moduleWidth = Math.Max(1, barcodeDefaults?.ModuleWidth ?? 2);
        var barHeight = Math.Max(1, barcode.Height > 0 ? barcode.Height : barcodeDefaults?.Height ?? 10);
        var displayContent = ResolveBarcodeContent(barcode);
        var content = NormalizeCodabarContent(displayContent);
        content = ApplyCodabarChecksum(content, barcode.CheckDigit);
        content =
            $"{char.ToUpperInvariant(barcode.StartCharacter)}{content}{char.ToUpperInvariant(barcode.StopCharacter)}";
        if (string.IsNullOrWhiteSpace(content))
        {
            warnings.Add("Codabar barcode skipped because content is empty.");
            return;
        }

        var foreground = state.ReverseField ? state.Background : state.Foreground;
        var background = state.ReverseField ? state.Foreground : state.Background;

        var bitmap = CreateZintLinearBitmap(
            ZintSupport.BarcodeType.BARCODE_CODABAR,
            content,
            moduleWidth,
            barHeight,
            barcode.PrintInterpretationLine,
            barcode.Orientation.ToString(),
            foreground,
            background);
        DrawBitmap(graphics, ResolveOrigin(barcode.Origin, state), state, bitmap);
        state.PendingBarcodeText = displayContent;
        state.PendingBarcodeOrigin = ResolveOrigin(barcode.Origin, state);
        state.PendingBarcodeBounds = new Rectangle(
            state.OffsetX + (ResolveOrigin(barcode.Origin, state)?.PositionX ?? 0),
            state.OffsetY + (ResolveOrigin(barcode.Origin, state)?.PositionY ?? 0), bitmap.Width, bitmap.Height);
        bitmap.Dispose();
    }

    private static void DrawQrCode(Graphics graphics, BarcodeQR barcode, string content, RenderState state, List<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            warnings.Add("QR code skipped because content is empty.");
            return;
        }

        var foreground = state.ReverseField ? state.Background : state.Foreground;
        var background = state.ReverseField ? state.Foreground : state.Background;
        var bitmap = CreateZintMatrixBitmap(
            barcode.Model == 1 ? ZintSupport.BarcodeType.BARCODE_MICROQR : ZintSupport.BarcodeType.BARCODE_QRCODE,
            content,
            0,
            MapQrErrorCorrection(barcode.ErrorCorrection),
            Math.Max(1, barcode.MagnificationFactor),
            MapOrientation(barcode.FieldPosition),
            IsGs1Payload(content),
            barcode.FieldPosition,
            foreground,
            background);
        DrawBitmap(graphics, ResolveOrigin(barcode.Origin, state), state, bitmap);
        state.PendingBarcodeText = null;
        state.PendingBarcodeOrigin = null;
        state.PendingBarcodeBounds = null;
        bitmap.Dispose();
    }

    private static void DrawDataMatrix(Graphics graphics, BarcodeDatamatrix barcode, string content, RenderState state,
        List<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            warnings.Add("Data Matrix skipped because content is empty.");
            return;
        }

        var foreground = state.ReverseField ? state.Background : state.Foreground;
        var background = state.ReverseField ? state.Foreground : state.Background;
        var bitmap = CreateZintMatrixBitmap(
            ZintSupport.BarcodeType.BARCODE_DATAMATRIX,
            content,
            MapDataMatrixVersionOption(barcode.Cols, barcode.Rows),
            0,
            Math.Max(1, barcode.DMHeight),
            MapOrientation(barcode.Orientation),
            IsGs1Payload(content),
            barcode.Orientation,
            foreground,
            background);
        DrawBitmap(graphics, ResolveOrigin(barcode.Origin, state), state, bitmap);
        state.PendingBarcodeText = null;
        state.PendingBarcodeOrigin = null;
        state.PendingBarcodeBounds = null;
        bitmap.Dispose();
    }

    private static void DrawBitmap(Graphics graphics, FieldOrigin? origin, RenderState state, Bitmap bitmap)
    {
        var x = state.OffsetX + (origin?.PositionX ?? 0);
        var y = state.OffsetY + (origin?.PositionY ?? 0);
        graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        graphics.DrawImage(bitmap, new Rectangle(x, y, bitmap.Width, bitmap.Height));
    }

    private static void DrawImage(Graphics graphics, FieldOrigin? origin, RenderState state, Image? image, int width,
        int height)
    {
        var x = state.OffsetX + (origin?.PositionX ?? 0);
        var y = state.OffsetY + (origin?.PositionY ?? 0);
        var rect = new Rectangle(x, y, Math.Max(1, width), Math.Max(1, height));
        if (image != null)
        {
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.DrawImage(image, rect);
        }
    }

    private static Font CreateFont(ScalableBitmappedFont? font, bool bold)
    {
        var rawSize = font?.FontHeight ?? 24f;
        var isFontA = string.Equals(font?.FontName, "A", StringComparison.OrdinalIgnoreCase);
        var size = Math.Max(8f, isFontA ? rawSize * 0.8f : rawSize * 0.9f);
        var family = isFontA
            ? "Courier New"
            : "Arial";
        return new Font(family, size, bold ? FontStyle.Bold : FontStyle.Regular, GraphicsUnit.Pixel);
    }

    private static int EstimateTextWidth(string? text, ScalableBitmappedFont? font)
    {
        var length = Math.Max(1, text?.Length ?? 1);
        var baseWidth = Math.Max(8, font?.FontWidth ?? 16);
        var factor = string.Equals(font?.FontName, "A", StringComparison.OrdinalIgnoreCase) ? baseWidth : baseWidth / 2;
        return Math.Max(120, length * Math.Max(8, factor));
    }

    private static Rectangle MeasureTextRect(Graphics graphics, string? text, Font font, int x, int y)
    {
        var value = text ?? string.Empty;
        var measured = TextRenderer.MeasureText(graphics, value, font, new Size(int.MaxValue, int.MaxValue),
            TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.NoPrefix);
        var width = Math.Max(1, measured.Width + 4);
        var height = Math.Max(1, measured.Height + 4);
        return new Rectangle(x, y, width, height);
    }

    private static string ApplyCode39Checksum(string content, bool appendChecksum)
    {
        if (!appendChecksum)
            return content;

        const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
        var sum = 0;
        foreach (var character in content.ToUpperInvariant())
        {
            var index = alphabet.IndexOf(character);
            if (index >= 0)
                sum += index;
        }

        return content + alphabet[sum % 43];
    }

    private static string ApplyCodabarChecksum(string content, bool appendChecksum)
    {
        if (!appendChecksum)
            return content;

        const string alphabet = "0123456789-$:/.+ABCD";
        var sum = 0;
        foreach (var character in content.ToUpperInvariant())
        {
            var index = alphabet.IndexOf(character);
            if (index >= 0)
                sum += index;
        }

        return content + alphabet[sum % 16];
    }

    private static string NormalizeCodabarContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        var builder = new StringBuilder(content.Length);
        foreach (var character in content)
        {
            if (char.IsDigit(character) || "-$:/.+".IndexOf(character) >= 0)
                builder.Append(character);
        }

        return builder.ToString();
    }

    private static bool IsGs1Payload(string? content)
    {
        if (string.IsNullOrEmpty(content))
            return false;

        var payload = content ?? string.Empty;
        return payload.IndexOf((char)29) >= 0 ||
               payload.IndexOf("]C1", StringComparison.OrdinalIgnoreCase) >= 0 ||
               payload.IndexOf("]Q3", StringComparison.OrdinalIgnoreCase) >= 0 ||
               payload.IndexOf("]d2", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static int EstimateBarcodeWidth(string? content, int moduleWidth, int quietZone)
    {
        var length = Math.Max(1, content?.Length ?? 1);
        return Math.Max(160, length * moduleWidth * 14 + quietZone * 2);
    }

    private static int MapQrErrorCorrection(Enums.ErrorCorrection errorCorrection)
    {
        return errorCorrection switch
        {
            Enums.ErrorCorrection.M => 2,
            Enums.ErrorCorrection.Q => 3,
            Enums.ErrorCorrection.H => 4,
            _ => 1
        };
    }

    private static Bitmap CreateZintLinearBitmap(int symbology, string content, int moduleWidth, int barHeight,
        bool showReadableLine, string orientation, Color foreground, Color background, bool gs1 = false)
    {
        var effectiveScale = Math.Max(1, moduleWidth);
        var zintHeight = Math.Max(1, (barHeight / effectiveScale) - 9);
        var command = new ZintSupport.ZintCmd
        {
            BarcodeType = symbology,
            HasReadableLine = showReadableLine,
            Height = zintHeight,
            MinimumHeight = zintHeight,
            Option_1 = 0,
            Option_2 = 0,
            InputMode = gs1 ? ZintSupport.InputMode.GS1_MODE : ZintSupport.InputMode.DATA_MODE,
            ScaleInput = effectiveScale,
            ScaleOutput = 1,
            Value = content,
            Encoding = Encoding.GetEncoding(1252),
            Angle = OrientationToAngle(orientation)
        };

        var symbol = new ZintSupport.Zint.zint_symbol();
        var bitmap = command.RefreshBarcode(ref symbol);
        if (foreground.ToArgb() != Color.Black.ToArgb() || background.ToArgb() != Color.White.ToArgb())
            bitmap = RecolorBitmap(bitmap, foreground, background);
        return bitmap;
    }

    private static Bitmap CreateZintMatrixBitmap(int symbology, string content, int option2, int option1, int scale,
        int angle, bool gs1, string orientation, Color foreground, Color background)
    {
        var command = new ZintSupport.ZintCmd
        {
            BarcodeType = symbology,
            HasReadableLine = false,
            Height = 0,
            MinimumHeight = 0,
            Option_1 = option1,
            Option_2 = option2,
            InputMode = gs1 ? ZintSupport.InputMode.GS1_MODE : ZintSupport.InputMode.DATA_MODE,
            ScaleInput = 1,
            ScaleOutput = Math.Max(1, scale),
            Value = content,
            Encoding = Encoding.GetEncoding(1252),
            Angle = angle
        };

        var symbol = new ZintSupport.Zint.zint_symbol();
        var bitmap = command.RefreshBarcode(ref symbol);
        if (foreground.ToArgb() != Color.Black.ToArgb() || background.ToArgb() != Color.White.ToArgb())
            bitmap = RecolorBitmap(bitmap, foreground, background);
        return bitmap;
    }

    private static Bitmap RecolorBitmap(Bitmap bitmap, Color foreground, Color background)
    {
        if (foreground.ToArgb() == Color.Black.ToArgb() && background.ToArgb() == Color.White.ToArgb())
            return bitmap;

        var recolored = new Bitmap(bitmap.Width, bitmap.Height);
        using (var graphics = Graphics.FromImage(recolored))
        {
            graphics.Clear(background);
            using var attributes = new ImageAttributes();
            var matrix = new ColorMatrix(new[]
            {
                new[] {(background.R - foreground.R) / 255f, 0f, 0f, 0f, foreground.R / 255f},
                new[] {0f, (background.G - foreground.G) / 255f, 0f, 0f, foreground.G / 255f},
                new[] {0f, 0f, (background.B - foreground.B) / 255f, 0f, foreground.B / 255f},
                new[] {0f, 0f, 0f, 1f, 0f},
                new[] {0f, 0f, 0f, 0f, 1f}
            });
            attributes.SetColorMatrix(matrix);
            graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width,
                bitmap.Height, GraphicsUnit.Pixel, attributes);
        }

        bitmap.Dispose();
        return recolored;
    }

    private static int OrientationToAngle(string? orientation)
    {
        return (orientation ?? string.Empty).Trim().ToUpperInvariant() switch
        {
            "R" => 90,
            "I" => 180,
            "B" => 270,
            _ => 0
        };
    }

    private static int MapOrientation(string? orientation) => OrientationToAngle(orientation);

    private static int MapQrVersionOption(int cols, int rows)
    {
        var sizes = new[]
        {
            "Auto",
            "21x21",
            "25x25",
            "29x29",
            "33x33",
            "37x37",
            "41x41",
            "45x45",
            "49x49",
            "53x53",
            "57x57",
            "61x61",
            "65x65",
            "69x69",
            "73x73",
            "77x77",
            "81x81",
            "85x85",
            "89x89",
            "93x93",
            "97x97",
            "101x101",
            "105x105",
            "109x109",
            "113x113",
            "117x117",
            "121x121",
            "125x125",
            "129x129",
            "133x133",
            "137x137",
            "141x141",
            "145x145",
            "149x149",
            "153x153",
            "157x157",
            "161x161",
            "165x165",
            "169x169",
            "173x173",
            "177x177"
        };

        var wanted = $"{cols}x{rows}";
        for (var index = 1; index < sizes.Length; index++)
        {
            if (string.Equals(sizes[index], wanted, StringComparison.OrdinalIgnoreCase))
                return index;
        }

        return 0;
    }

    private static int MapDataMatrixVersionOption(int cols, int rows)
    {
        var sizes = new[]
        {
            "Auto", "10x10", "12x12", "14x14", "16x16", "18x18", "20x20", "22x22", "24x24", "26x26",
            "32x32", "36x36", "40x40", "44x44", "48x48", "52x52"
        };

        var wanted = $"{cols}x{rows}";
        for (var index = 1; index < sizes.Length; index++)
        {
            if (string.Equals(sizes[index], wanted, StringComparison.OrdinalIgnoreCase))
                return index;
        }

        return 0;
    }

    private static void ApplyOrientation(Bitmap bitmap, Enums.Orientation orientation)
    {
        switch (orientation)
        {
            case Enums.Orientation.R:
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                break;
            case Enums.Orientation.I:
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                break;
            case Enums.Orientation.B:
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                break;
        }
    }

    private static void ApplyOrientation(Bitmap bitmap, string orientation)
    {
        switch ((orientation ?? string.Empty).ToUpperInvariant())
        {
            case "R":
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                break;
            case "I":
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                break;
            case "B":
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                break;
        }
    }

    private static string GraphicKey(string storageDevice, string imageName, string extension)
    {
        return $"{storageDevice}|{imageName}|{extension}".ToUpperInvariant();
    }

    private static FieldOrigin? ResolveOrigin(FieldOrigin? origin, RenderState state)
    {
        return origin ?? state.PendingFieldOrigin;
    }

    private static string ResolveBarcodeContent(BaseElement element)
    {
        if (element is Barcode1D barcode1D)
        {
            if (!string.IsNullOrWhiteSpace(barcode1D.Content))
                return barcode1D.Content;

            if (element.Child is FieldData fieldData)
                return fieldData.Data ?? string.Empty;
        }

        if (element is BarcodeQR barcodeQR)
        {
            if (!string.IsNullOrWhiteSpace(barcodeQR.Content))
                return barcodeQR.Content;

            if (element.Child is FieldData fieldData)
                return ExtractStructuredBarcodeContent(fieldData.Data);
        }

        if (element is BarcodeDatamatrix barcodeDatamatrix)
        {
            if (!string.IsNullOrWhiteSpace(barcodeDatamatrix.Content))
                return barcodeDatamatrix.Content;

            if (element.Child is FieldData fieldData)
                return ExtractStructuredBarcodeContent(fieldData.Data);
        }

        return string.Empty;
    }

    private static string ExtractStructuredBarcodeContent(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        var trimmed = data.Trim();
        var commaIndex = trimmed.IndexOf(',');
        return commaIndex >= 0 && commaIndex + 1 < trimmed.Length
            ? trimmed.Substring(commaIndex + 1)
            : trimmed;
    }

    private sealed class RenderState
    {
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public BarcodeFieldDefault? BarcodeDefaults { get; set; }
        public bool ReverseField { get; set; }
        public FieldOrigin? PendingFieldOrigin { get; set; }
        public string? PendingBarcodeText { get; set; }
        public FieldOrigin? PendingBarcodeOrigin { get; set; }
        public Rectangle? PendingBarcodeBounds { get; set; }
        public BarcodeQR? PendingQrBarcode { get; set; }
        public BarcodeDatamatrix? PendingDataMatrixBarcode { get; set; }
        public bool SuppressNextTextField { get; set; }
    }
}
