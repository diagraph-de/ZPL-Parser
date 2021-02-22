using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class ScalableBitmappedFont : BaseElement
    {
        private static ScalableBitmappedFont _currentFont;
        private readonly string properties;
        private byte[] elementBytes;

        public ScalableBitmappedFont(string fontName, string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            FontName = fontName.Replace(Environment.NewLine, "");

            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');
            switch (sp[0].ToUpper())
            {
                case "N":
                    Orientation = Enums.Orientation.N; //normal
                    break;
                case "R":
                    Orientation = Enums.Orientation.R; //rotated 90 degrees clockwise
                    break;
                case "I":
                    Orientation = Enums.Orientation.I; //inverted 180 degrees
                    break;
                case "B":
                    Orientation = Enums.Orientation.B; //read from bottom up 270 degrees
                    break;
            }
            FontWidth = Convert.ToInt32(sp[1]);
            FontHeight = Convert.ToInt32(sp[2]);
        }

        public ScalableBitmappedFont(int fontWidth = 30, int fontHeight = 30, string fontName = "0", Enums.Orientation orientation = Enums.Orientation.N)
        {
            FontName = fontName;
            Orientation = orientation;
            FontWidth = fontWidth;
            FontHeight = fontHeight;
        }

        public static ScalableBitmappedFont Current
        {
            get { return _currentFont ?? (_currentFont = new ScalableBitmappedFont()); }
            set { _currentFont = value; }
        }

        public string FontName { get; set; }
        public Enums.Orientation Orientation { get; set; }
        public int FontWidth { get; set; }
        public int FontHeight { get; set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var textOrientation = Orientation;
            if (string.IsNullOrEmpty(Orientation.ToString()))
                textOrientation = context.DefaultTextOrientation;

            var result = new List<string>();
            result.Add("^A" + FontName + textOrientation + "," + context.Scale(FontHeight) + "," + context.Scale(FontWidth));
            return result;
        }
    }
}