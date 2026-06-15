#nullable enable

using System.Collections.Generic;
using System.Drawing;

namespace Diagraph.Labelparser.ZPL;

public sealed class PreviewRenderResult
{
    public Bitmap? PreviewBitmap { get; set; }
    public string Tree { get; set; } = string.Empty;
    public string NormalizedZpl { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public List<PreviewElementInfo> Elements { get; } = new();
    public int CanvasWidth { get; set; }
    public int CanvasHeight { get; set; }
}

public sealed class PreviewSurfaceSettings
{
    public static PreviewSurfaceSettings Default { get; } = new();
    public int PrintDensityDpmm { get; set; } = 8;
    public double LabelWidthInches { get; set; } = 4;
    public double LabelHeightInches { get; set; } = 6;
    public int LabelIndex { get; set; }
}

public sealed class PreviewElementInfo
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}