#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using TextEncoding = System.Text.Encoding;

#endregion

namespace Diagraph.Labelparser.ZPL.ZintSupport;

public sealed class ZintCmd : IDisposable
{
    public int BarcodeType { get; set; }
    public int Rotate { get; set; }
    public int ScaleInput { get; set; }
    public int ScaleOutput { get; set; }
    public int Option_1 { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public int Option_2 { get; set; }
    public int Height { get; set; }
    public int MinimumHeight { get; set; }
    public int InputMode { get; set; }
    public TextEncoding Encoding { get; set; } = TextEncoding.GetEncoding(1252);
    public bool HasReadableLine { get; set; }
    public string ReadableLine { get; private set; } = string.Empty;
    public bool HasBorder { get; set; }
    public bool SupressErrorMessage { get; set; }
    public int SegmentsPerRow { get; set; }
    public string PrimaryValue { get; set; } = string.Empty;
    public int Angle { get; set; }

    public void Dispose()
    {
    }

    public Bitmap RefreshBarcode(ref Zint.zint_symbol symbol)
    {
        var native = Zint.Create();
        if (native == IntPtr.Zero)
            return new Bitmap(1, 1);

        var nativeSymbol = Marshal.PtrToStructure<Zint.zint_symbol>(native);
        try
        {
            var inputValue = PrepareInput(Value ?? string.Empty);
            var displayValue = PrepareDisplayValue(inputValue);
            var inputEncoding = Encoding ?? TextEncoding.GetEncoding(1252);
            var bytes = inputEncoding.GetBytes(inputValue);

            nativeSymbol.symbology = BarcodeType;
            nativeSymbol.height = Math.Max(0, Math.Max(Height, MinimumHeight));
            nativeSymbol.option_1 = Option_1;
            nativeSymbol.option_2 = Option_2;
            nativeSymbol.option_3 = 0;
            nativeSymbol.show_hrt = HasReadableLine ? 1 : 0;
            nativeSymbol.fontsize = 0;
            nativeSymbol.input_mode = InputMode;
            nativeSymbol.scale = ScaleInput > 0 ? ScaleInput : 1;
            nativeSymbol.eci = GetEci(inputEncoding);
            nativeSymbol.text = HasReadableLine ? displayValue : string.Empty;
            nativeSymbol.primary = PrimaryValue ?? string.Empty;
            nativeSymbol.border_width = HasBorder ? 1 : 0;
            if (SegmentsPerRow > 0)
                nativeSymbol.option_2 = SegmentsPerRow;

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                var result = Zint.EncodeAndBuffer(ref nativeSymbol, handle.AddrOfPinnedObject(), bytes.Length,
                    Angle != 0 ? Angle : Rotate);
                if (result != 0 && string.IsNullOrWhiteSpace(nativeSymbol.errtxt))
                    throw new InvalidOperationException($"Zint encoding failed with code {result}.");
            }
            finally
            {
                handle.Free();
            }

            ReadableLine = HasReadableLine ? displayValue : string.Empty;
            symbol = nativeSymbol;

            if (nativeSymbol.bitmap == IntPtr.Zero || nativeSymbol.bitmap_width <= 0 || nativeSymbol.bitmap_height <= 0)
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(nativeSymbol.errtxt)
                    ? "Zint returned an empty bitmap."
                    : nativeSymbol.errtxt);

            var bitmap = CopyBitmap(nativeSymbol.bitmap, nativeSymbol.bitmap_width, nativeSymbol.bitmap_height,
                nativeSymbol.bitmap_byte_length);
            if (ScaleOutput > 0)
                bitmap = ResizeImage(bitmap, Math.Max(1, bitmap.Width * ScaleOutput / 2),
                    Math.Max(1, bitmap.Height * ScaleOutput / 2));

            return bitmap;
        }
        catch
        {
            ReadableLine = string.Empty;
            symbol = nativeSymbol;
            if (!SupressErrorMessage && !string.IsNullOrWhiteSpace(nativeSymbol.errtxt))
                throw new InvalidOperationException(nativeSymbol.errtxt);
            return new Bitmap(1, 1);
        }
        finally
        {
            // Intentionally do not call the native delete routine here.
            // The marshaled symbol copy is not guaranteed to match the exact
            // native allocation layout that the C API expects, and deleting it
            // can corrupt the heap in the host process.
        }
    }

    private static Bitmap CopyBitmap(IntPtr bitmapPointer, int width, int height, uint byteLength)
    {
        var bytesToCopy = (int)Math.Min(int.MaxValue, Math.Max(byteLength, (uint)(width * height * 3)));
        var buffer = new byte[bytesToCopy];
        Marshal.Copy(bitmapPointer, buffer, 0, buffer.Length);

        var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
        try
        {
            var rowLength = width * 3;
            var stride = Math.Abs(data.Stride);
            for (var row = 0; row < height; row++)
            {
                var sourceOffset = row * rowLength;
                if (sourceOffset + rowLength > buffer.Length)
                    break;

                var destinationRow = data.Stride > 0 ? row : height - row - 1;
                var destination = IntPtr.Add(data.Scan0, destinationRow * stride);
                Marshal.Copy(buffer, sourceOffset, destination, rowLength);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return bitmap;
    }

    public static Bitmap ResizeImage(Bitmap image, int width, int height, int angle = 0)
    {
        var resizedImage = new Bitmap(width, height);
        using (var graphics = Graphics.FromImage(resizedImage))
        {
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, 0, image.Width, image.Height),
                GraphicsUnit.Pixel);
        }

        switch (angle)
        {
            case 90:
                resizedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                break;
            case 180:
                resizedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                break;
            case 270:
                resizedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                break;
        }

        return resizedImage;
    }

    private static int GetEci(TextEncoding encoding)
    {
        return encoding.CodePage switch
        {
            65001 => (int)Zint.ECI.Unicode_UTF8,
            1250 => (int)Zint.ECI.Windows1250_Latin2CentralEurope,
            1251 => (int)Zint.ECI.Windows1251_Cyrillic,
            1252 => (int)Zint.ECI.Windows1252_Latin1,
            1256 => (int)Zint.ECI.Windows1256_Arabic,
            _ => (int)Zint.ECI.Windows1252_Latin1
        };
    }

    private static string PrepareInput(string value)
    {
        var normalized = ReplaceSpecialChars(value ?? string.Empty);
        normalized = normalized.Replace("\r", string.Empty).Replace("\n", string.Empty);
        normalized = NormalizeGs1Input(normalized);
        return normalized;
    }

    private static string PrepareDisplayValue(string value)
    {
        return value.Replace("[", "(").Replace("]", ")");
    }

    private static string NormalizeGs1Input(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var result = value.Replace("{FNC1}", string.Empty)
            .Replace("\u001d", string.Empty)
            .Trim();

        if (result.Contains("(") || result.Contains(")"))
            result = result.Replace("(", "[").Replace(")", "]");

        return result;
    }

    private static string ReplaceSpecialChars(string value, bool skipfnc1 = false)
    {
        var replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!skipfnc1)
            replacements["FNC1"] = string.Empty;
        replacements["FNC2"] = string.Empty;
        replacements["FNC3"] = string.Empty;
        replacements["FNC4"] = string.Empty;
        replacements["NUL"] = ((char)0).ToString();
        replacements["SOH"] = ((char)1).ToString();
        replacements["STX"] = ((char)2).ToString();
        replacements["ETX"] = ((char)3).ToString();
        replacements["EOT"] = ((char)4).ToString();
        replacements["ENQ"] = ((char)5).ToString();
        replacements["ACK"] = ((char)6).ToString();
        replacements["BEL"] = ((char)7).ToString();
        replacements["HT"] = ((char)8).ToString();
        replacements["BS"] = ((char)9).ToString();
        replacements["LF"] = ((char)10).ToString();
        replacements["VT"] = ((char)11).ToString();
        replacements["FF"] = ((char)12).ToString();
        replacements["CR"] = ((char)13).ToString();
        replacements["SO"] = ((char)14).ToString();
        replacements["SI"] = ((char)15).ToString();
        replacements["DLE"] = ((char)16).ToString();
        replacements["DC1"] = ((char)17).ToString();
        replacements["DC2"] = ((char)18).ToString();
        replacements["DC3"] = ((char)19).ToString();
        replacements["DC4"] = ((char)20).ToString();
        replacements["NAK"] = ((char)21).ToString();
        replacements["SYN"] = ((char)22).ToString();
        replacements["ETB"] = ((char)23).ToString();
        replacements["CAN"] = ((char)24).ToString();
        replacements["EM"] = ((char)25).ToString();
        replacements["SUB"] = ((char)26).ToString();
        replacements["ESC"] = ((char)27).ToString();
        replacements["FS"] = ((char)28).ToString();
        replacements["GS"] = ((char)29).ToString();
        replacements["RS"] = ((char)30).ToString();
        replacements["US"] = ((char)31).ToString();
        replacements["DEL"] = ((char)127).ToString();

        foreach (var replacement in replacements)
            value = value.Replace("{" + replacement.Key + "}", replacement.Value);

        return value;
    }
}
