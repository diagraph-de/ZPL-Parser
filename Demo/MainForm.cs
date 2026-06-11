using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Diagraph.Labelparser.ZPL;

namespace ZPLParser.Demo;

internal sealed partial class MainForm : Form
{
    private OpenFileDialog? _openFileDialog;
    private Timer? _refreshTimer;
    private ZplPreviewRenderer? _renderer;
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
        _refreshTimer = new Timer { Interval = 250 };
        _renderer = new ZplPreviewRenderer();
        _elementsGrid.DataSource = new BindingSource();
        _zplInput.TextChanged += (_, __) => RestartRefreshTimer();
        _overviewButton.Click += OverviewButton_Click;
        _graphicsButton.Click += GraphicsButton_Click;
        _barcodeMixButton.Click += BarcodeMixButton_Click;
        _openZplButton.Click += OpenZplButton_Click;
        _refreshButton.Click += RefreshButton_Click;
        _copyZplButton.Click += CopyZplButton_Click;
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

    private void LoadSample(string zpl)
    {
        _zplInput.Text = zpl;
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

        SelectZoom("100%");
        LoadSample(BuildOverviewSample());
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

        var result = _renderer.Render(_zplInput.Text);
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
