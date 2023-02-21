namespace Allen.Labelparser.ZPL
{
    /// <summary>
    ///     Holding redering settings
    /// </summary>
    public class ZPLRenderOptions
    {
        public static ZPLRenderOptions DefaultOptions = new ZPLRenderOptions();

        public ZPLRenderOptions()
        {
            ChangeInternationalFontEncoding = ZPLConstants.InternationalFontEncoding.CI28;
            SourcePrintDPI = TargetPrintDPI = 203;
            DefaultTextOrientation = Enums.Orientation.N;
        }

        //^CI
        public string ChangeInternationalFontEncoding { get; set; }

        public Enums.Orientation DefaultTextOrientation { get; set; }

        public bool DisplayComments { get; set; }

        public bool AddEmptyLineBeforeElementStart { get; set; }

        public int SourcePrintDPI { get; set; }

        public int TargetPrintDPI { get; set; }

        public bool CompressedRendering { get; set; }

        public double ScaleFactor => (double) TargetPrintDPI / SourcePrintDPI;

        public int Scale(int input)
        {
            return (int) (input * ScaleFactor);
        }

        public double Scale(double input)
        {
            return input * ScaleFactor;
        }
    }
}