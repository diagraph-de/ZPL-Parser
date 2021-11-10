using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class GraphicDiagonalLine : GraphicBox
    {
        private readonly string properties;
        private byte[] elementBytes;

        public GraphicDiagonalLine(int positionX, int positionY, int width, int height, int borderThickness = 1,
            bool rightLeaningiagonal = false,
            Enums.BlackWhite lineColor = Enums.BlackWhite.B, int cornerRounding = 0) : base(positionX, positionY, width,
            height, borderThickness, lineColor)
        {
            Base = typeof(GraphicElement);
            RightLeaningiagonal = rightLeaningiagonal;
        }

        public GraphicDiagonalLine(string properties, byte[] elementBytes)
        {
            Base = typeof(GraphicElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            AddFieldSeparator = false;
            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');

            if (sp.Length > 0)
                Width = Convert.ToInt32(sp[0]);

            if (sp.Length > 1)
                Height = Convert.ToInt32(sp[1]);

            if (sp.Length > 2)
                BorderThickness = Convert.ToInt32(sp[2]);

            if (sp.Length > 3)
                LineColor = sp[3].ToUpper() == Enums.BlackWhite.W.ToString()
                    ? Enums.BlackWhite.W
                    : Enums.BlackWhite.B;

            if (sp.Length > 4)
                RightLeaningiagonal = sp[4].ToUpper() == Enums.DiagonalOrientation.R.ToString();
        }

        public bool RightLeaningiagonal { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^GDw,h,t,c,o
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^GD" + context.Scale(Width) + "," + context.Scale(Height) + "," +
                       context.Scale(BorderThickness) + "," + LineColor + "," +
                       (RightLeaningiagonal
                           ? "R"
                           : "L"));
            if (AddFieldSeparator)
                result.Add("^FS");

            return result;
        }
    }
}