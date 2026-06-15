using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Diagraph.Labelparser.ZPL;

namespace ZPLParser.Demo;

internal sealed partial class MainForm : Form
{
    private readonly OpenFileDialog? _openFileDialog;
    private readonly Timer? _refreshTimer;
    private readonly ZplPreviewRenderer? _renderer;
    private bool _updatingHighlighting;
    private bool _updatingZoom;

    public MainForm()
    {
        InitializeComponent();
        if (IsDesignTime())
            return;

        _openFileDialog = new OpenFileDialog
        {
            Filter = "ZPL files (*.zpl;*.txt)|*.zpl;*.txt|All files (*.*)|*.*",
            Title = "Open ZPL file"
        };
        _refreshTimer = new Timer { Interval = 500 };
        _renderer = new ZplPreviewRenderer();
        _elementsGrid.DataSource = new BindingSource();
        _zplInput.TextChanged += (_, __) =>
        {
            ApplyZplSyntaxHighlighting();
            RestartRefreshTimer();
        };
        _overviewButton.Click += OverviewButton_Click;
        _graphicsButton.Click += GraphicsButton_Click;
        _barcodeMixButton.Click += BarcodeMixButton_Click;
        _labelButton.Click += LabelButton_Click;
        _openZplButton.Click += OpenZplButton_Click;
        _refreshButton.Click += RefreshButton_Click;
        _copyZplButton.Click += CopyZplButton_Click;
        _printDensityComboBox.SelectedIndexChanged += SettingsChanged;
        _printQualityComboBox.SelectedIndexChanged += SettingsChanged;
        _labelWidthTextBox.TextChanged += SettingsChanged;
        _labelHeightTextBox.TextChanged += SettingsChanged;
        _labelUnitComboBox.SelectedIndexChanged += SettingsChanged;
        _showLabelIndexNumeric.ValueChanged += SettingsChanged;
        _showLabelTotalNumeric.ValueChanged += SettingsChanged;
        _rememberLastLabelCheckBox.CheckedChanged += SettingsChanged;
        _zoomComboBox.SelectedIndexChanged += ZoomComboBox_SelectedIndexChanged;
        _fitToWindowCheckBox.CheckedChanged += FitToWindowCheckBox_CheckedChanged;
        _previewHost.Resize += PreviewHost_Resize;
        Load += MainForm_Load;
        FormClosed += MainForm_FormClosed;
        _refreshTimer.Tick += (_, __) =>
        {
            _refreshTimer.Stop();
            RefreshPreview();
        };
    }

    private static bool IsDesignTime()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    }

    private void RestartRefreshTimer()
    {
        if (_refreshTimer == null)
            return;

        _refreshTimer.Stop();
        _refreshTimer.Start();
    }

    private void SettingsChanged(object? sender, EventArgs e)
    {
        RestartRefreshTimer();
    }

    private void LoadSample(string zpl)
    {
        _zplInput.Text = zpl;
        ApplyZplSyntaxHighlighting();
    }

    private void OpenZplFile()
    {
        if (_openFileDialog == null)
            return;

        if (_openFileDialog.ShowDialog(this) == DialogResult.OK)
            LoadSample(File.ReadAllText(_openFileDialog.FileName));
    }

    private static int ParseZoom(string? label)
    {
        if (label == "Fit")
            return 100;

        return int.TryParse(label?.TrimEnd('%'), out var value) ? value : 100;
    }

    private void SelectZoom(string value)
    {
        _updatingZoom = true;
        _zoomComboBox.SelectedItem = value;
        _updatingZoom = false;

        if (value == "Fit")
            _fitToWindowCheckBox.Checked = true;
        else
            _fitToWindowCheckBox.Checked = false;

        ApplyZoom(ParseZoom(value));
    }

    private void ApplyZoom(int zoomPercent)
    {
        if (_previewPictureBox.Image == null)
            return;

        var fitToWindow = _fitToWindowCheckBox.Checked;
        if (fitToWindow)
        {
            var availableWidth = Math.Max(100, _previewHost.ClientSize.Width - 32);
            var availableHeight = Math.Max(100, _previewHost.ClientSize.Height - 32);
            var scale = Math.Min(
                (double)availableWidth / _previewPictureBox.Image.Width,
                (double)availableHeight / _previewPictureBox.Image.Height);
            if (double.IsNaN(scale) || double.IsInfinity(scale) || scale <= 0)
                scale = 1;

            zoomPercent = (int)Math.Round(scale * 100);
        }

        zoomPercent = Math.Max(10, zoomPercent);
        var scaledWidth = Math.Max(1, (int)Math.Round(_previewPictureBox.Image.Width * zoomPercent / 100.0));
        var scaledHeight = Math.Max(1, (int)Math.Round(_previewPictureBox.Image.Height * zoomPercent / 100.0));
        _previewPictureBox.Size = new Size(scaledWidth, scaledHeight);
        _previewPictureBox.Location = new Point(16, 16);
        _previewHost.AutoScrollMinSize = new Size(scaledWidth + 32, scaledHeight + 32);
        _previewPictureBox.Invalidate();

        if (_zoomComboBox != null && !_updatingZoom)
        {
            var label = zoomPercent + "%";
            if (_zoomComboBox.Items.Contains(label))
                _zoomComboBox.SelectedItem = label;
        }
    }

    private void OverviewButton_Click(object? sender, EventArgs e)
    {
        LoadSample(BuildOverviewSample());
        RefreshPreview();
    }

    private void GraphicsButton_Click(object? sender, EventArgs e)
    {
        LoadSample(BuildGraphicsSample());
        RefreshPreview();
    }

    private void BarcodeMixButton_Click(object? sender, EventArgs e)
    {
        LoadSample(BuildBarcodeSample());
        RefreshPreview();
    }

    private void LabelButton_Click(object? sender, EventArgs e)
    {
        LoadSample(BuildReferenceLabelSample());
        RefreshPreview();
    }

    private void OpenZplButton_Click(object? sender, EventArgs e)
    {
        OpenZplFile();
    }

    private void RefreshButton_Click(object? sender, EventArgs e)
    {
        RefreshPreview();
    }

    private void CopyZplButton_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_normalizedZplTextBox.Text))
            Clipboard.SetText(_normalizedZplTextBox.Text);
    }

    private void ZoomComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_updatingZoom)
            return;

        var selectedValue = _zoomComboBox.SelectedItem as string;
        if (selectedValue == "Fit")
        {
            _fitToWindowCheckBox.Checked = true;
            ApplyZoom(100);
            return;
        }

        if (!string.IsNullOrWhiteSpace(selectedValue))
        {
            _fitToWindowCheckBox.Checked = false;
            ApplyZoom(ParseZoom(selectedValue));
        }
    }

    private void FitToWindowCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        if (_fitToWindowCheckBox.Checked)
            ApplyZoom(100);
        else
            ApplyZoom(ParseZoom(_zoomComboBox.SelectedItem?.ToString()));
    }

    private void PreviewHost_Resize(object? sender, EventArgs e)
    {
        if (_fitToWindowCheckBox.Checked)
            ApplyZoom(100);
    }

    private void MainForm_Load(object? sender, EventArgs e)
    {
        if (IsDesignTime())
            return;

        if (_renderer == null)
            return;

        _printDensityComboBox.SelectedIndex = 1;
        _printQualityComboBox.SelectedIndex = 0;
        _labelUnitComboBox.SelectedIndex = 0;
        _showLabelIndexNumeric.Value = 1;
        _showLabelTotalNumeric.Value = 1;
        SelectZoom("Fit");
        LoadSample(BuildReferenceLabelSample());
        RefreshPreview();
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _previewPictureBox.Image?.Dispose();
    }

    private void RefreshPreview()
    {
        if (_renderer == null)
            return;

        var result = _renderer.Render(_zplInput.Text, CreatePreviewSettings());
        ReplacePreviewImage(result.PreviewBitmap);
        ApplyZoom(_fitToWindowCheckBox?.Checked == true ? 100 : ParseZoom(_zoomComboBox?.SelectedItem?.ToString()));
        _normalizedZplTextBox.Text = result.NormalizedZpl;
        _treeTextBox.Text = result.Tree;
        ((BindingSource)_elementsGrid.DataSource!).DataSource = result.Elements;

        var statusParts = new List<string>
        {
            $"Elements: {result.Elements.Count}",
            $"Canvas: {result.CanvasWidth}×{result.CanvasHeight}"
        };

        if (!string.IsNullOrWhiteSpace(result.Error))
            statusParts.Add($"Warning: {result.Error}");

        _statusLabel.Text = string.Join(" | ", statusParts);
    }

    private PreviewSurfaceSettings CreatePreviewSettings()
    {
        var settings = new PreviewSurfaceSettings
        {
            PrintDensityDpmm = ParsePrintDensity(_printDensityComboBox.SelectedItem?.ToString()),
            LabelWidthInches = ParseLabelSize(_labelWidthTextBox.Text, _labelUnitComboBox.SelectedItem?.ToString()),
            LabelHeightInches = ParseLabelSize(_labelHeightTextBox.Text, _labelUnitComboBox.SelectedItem?.ToString()),
            LabelIndex = Math.Max(0, (int)_showLabelIndexNumeric.Value - 1)
        };

        return settings;
    }

    private static int ParsePrintDensity(string? value)
    {
        return value switch
        {
            string text when text.StartsWith("6 dpmm", StringComparison.OrdinalIgnoreCase) => 6,
            string text when text.StartsWith("12 dpmm", StringComparison.OrdinalIgnoreCase) => 12,
            string text when text.StartsWith("24 dpmm", StringComparison.OrdinalIgnoreCase) => 24,
            _ => 8
        };
    }

    private static double ParseLabelSize(string text, string? unit)
    {
        if (!double.TryParse(text, NumberStyles.Float,
                CultureInfo.InvariantCulture, out var value))
            value = 4;

        return unit switch
        {
            "mm" => value / 25.4,
            "cm" => value / 2.54,
            _ => value
        };
    }

    private void ApplyZplSyntaxHighlighting()
    {
        if (_updatingHighlighting || _zplInput == null)
            return;

        _updatingHighlighting = true;

        var selectionStart = _zplInput.SelectionStart;
        var selectionLength = _zplInput.SelectionLength;

        try
        {
            _zplInput.SuspendLayout();
            _zplInput.SelectAll();
            _zplInput.SelectionColor = Color.FromArgb(40, 40, 40);
            _zplInput.SelectionFont = new Font(_zplInput.Font, FontStyle.Regular);

            ApplyRegexStyle(@"(?m)^\^FX.*$", Color.FromArgb(0, 128, 0), FontStyle.Regular);
            ApplyRegexStyle(@"[\^~][A-Z0-9]{1,3}", Color.FromArgb(196, 52, 52), FontStyle.Bold);
            ApplyRegexStyle(@"(?<=\^FD).*?(?=\^FS)", Color.FromArgb(30, 30, 30), FontStyle.Regular);
            ApplyRegexStyle(@"(?m)^\s*$", Color.FromArgb(40, 40, 40), FontStyle.Regular);

            _zplInput.Select(selectionStart, selectionLength);
            _zplInput.SelectionColor = _zplInput.ForeColor;
        }
        finally
        {
            _zplInput.ResumeLayout();
            _updatingHighlighting = false;
        }
    }

    private void ApplyRegexStyle(string pattern, Color color, FontStyle fontStyle)
    {
        foreach (Match match in Regex.Matches(_zplInput.Text, pattern, RegexOptions.Multiline))
        {
            _zplInput.Select(match.Index, match.Length);
            _zplInput.SelectionColor = color;
            _zplInput.SelectionFont = new Font(_zplInput.Font, fontStyle);
        }
    }

    private void ReplacePreviewImage(Bitmap? bitmap)
    {
        var previous = _previewPictureBox.Image;
        _previewPictureBox.Image = bitmap;
        if (previous != null)
            previous.Dispose();
    }

    private string BuildOverviewSample()
    {
        var graphic = CreateDemoGraphic();
        var imageZpl = ImageHelper.ZPLfromBitmap(graphic, false);
        var engine = new ZPLEngine(new BaseElement[]
        {
            new PrintWidth(900),
            new LabelLength(820),
            new LabelHome(0, 0),
            new LabelTop(0),
            new LabelShfit(0),
            new PrintMode(),
            new Comment("Comprehensive sample with text, shapes, barcodes and graphics"),
            new ScalableBitmappedFont(40, 40),
            new TextField(40, 36, "ZPL Parser Demo", new ScalableBitmappedFont(42, 42)),
            new TextField(40, 94, "Live preview, parser tree, normalized ZPL and element list",
                new ScalableBitmappedFont(18, 18), useHexadecimalIndicator: false),
            new FieldBlock(40, 140, "Field block text wraps across multiple lines and shows the ^FB command.", 320,
                new ScalableBitmappedFont(18, 18), 3, 4),
            new TextBlock(40, 240, "Text block preview with ^TB support\nand multiple lines.", 320, 90,
                new ScalableBitmappedFont(18, 18), NewLineConversionMethod.ToZPLNewLine),
            new GraphicBox(400, 48, 180, 90, 4),
            new GraphicCircle(620, 48, 90, 4),
            new GraphicEllipse(740, 48, 120, 70, 4),
            new GraphicDiagonalLine(400, 170, 180, 90, 4, true),
            new GraphicSymbol(Enums.GraphicSymbolCharacter.Copyright, 640, 160, 56, 56),
            new BarcodeCode39(40, 380, "ZPL-39", 70),
            new BarcodeCode128(280, 380, "ZPL-128", 70),
            new BarcodeAnsiCodabar(520, 380, "123456789", 70, 'A', 'B'),
            new BarcodeQR(40, 530, "https://example.com/zpl"),
            new BarcodeDatamatrix(220, 520, "DATA-MATRIX"),
            new BaseDownloadGraphics('R', "DEMO", ".GRF", graphic),
            new RecallGraphic(620, 520, 'R', "DEMO", ".GRF", 2, 2),
            new BaseRaw("^FO40,720^GB820,2,2,B,0^FS")
        });

        return engine.ToZPLString(new ZPLRenderOptions { DisplayComments = true });
    }

    private string BuildGraphicsSample()
    {
        var graphic = CreateDemoGraphic();
        var imageZpl = ImageHelper.ZPLfromBitmap(graphic, false);

        return
            $@"^XA
^PW800
^LL520
^FO40,40^GB200,120,4,B,0^FS
^FO280,40^GC100,4,B^FS
^FO420,40^GD160,100,4,B,R^FS
^FO610,40^GE140,100,4,B^FS
^FO40,220{imageZpl.Result}^FS
^FO290,220^XGR:DEMO.GRF,2,2^FS
^FO40,380^FDGraphics sample: download, recall and direct graphic field^FS
^XZ";
    }

    private string BuildBarcodeSample()
    {
        return
            @"^XA
^PW760
^LL520
^FO40,40^A0N,36,36^FDBarcodes^FS
^FO40,100^B3N,N,80,Y,N^FDABC12345^FS
^FO40,230^BCN,90,Y,N,N^FD1234567890^FS
^FO40,360^BQN,2,7^FDLA,https://openai.com^FS
^FO340,230^BXN,8,200,0,0,1,_,1^FD7,DATA-MATRIX^FS
^FO340,360^BKN,Y,80,Y,N,A,A^FD1234-ABCD^FS
^XZ";
    }

    private static string BuildReferenceLabelSample()
    {
        return
            @"^XA

^FX Top section with logo, name and address.
^CF0,60
^FO50,50^GB100,100,100^FS
^FO75,75^FR^GB100,100,100^FS
^FO93,93^GB40,40,40^FS
^FO220,50^FDITW Diagraph GmbH^FS
^CF0,30
^FO220,115^FDFriedrich-Bergius-Ring 30^FS
^FO220,155^FD97076 Würzburg-Lengfeld^FS
^FO220,195^FDGermany^FS
^FO50,250^GB700,3,3^FS

^FX Second section with recipient address and permit information.
^CFA,30
^FO50,300^FDJohn Doe^FS
^FO50,340^FD1 Research Park Drive^FS
^FO50,380^FDSt. Charles, MO 63304-5685^FS
^FO50,420^FDUSA^FS
^CFA,15
^FO600,300^GB150,150,3^FS
^FO638,340^FDPermit^FS
^FO638,390^FD123456^FS
^FO50,500^GB700,3,3^FS

^FX Third section with bar code.
^BY5,2,270
^FO100,550^BC^FD12345678^FS

^FX Fourth section (the two boxes on the bottom).
^FO50,900^GB700,250,3^FS
^FO400,900^GB3,250,3^FS
^CF0,40
^FO100,960^FDCtr. X34B-1^FS
^FO100,1010^FDREF1 F00B47^FS
^FO100,1060^FDREF2 BL4H8^FS
^CF0,190
^FO470,955^FDCA^FS

^XZ";
    }

    private static Bitmap CreateDemoGraphic()
    {
        var bitmap = new Bitmap(120, 80, PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            using var pen = new Pen(Color.Black, 3);
            graphics.DrawRectangle(pen, 4, 4, 112, 72);
            graphics.DrawLine(pen, 8, 68, 108, 12);
            graphics.FillEllipse(Brushes.Black, 48, 18, 22, 22);
            using var font = new Font("Segoe UI", 10, FontStyle.Bold, GraphicsUnit.Point);
            graphics.DrawString("ZPL", font, Brushes.Black, new PointF(38, 44));
        }

        return bitmap;
    }
}