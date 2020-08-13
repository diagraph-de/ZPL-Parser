using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class GraphicEllipse : GraphicBox
    {
        private readonly string properties;
        private byte[] elementBytes;

        public GraphicEllipse(int positionX, int positionY, int width, int height, int borderThickness = 1, Enums.BlackWhite lineColor = Enums.BlackWhite.B) :
            base(positionX, positionY, width, height, borderThickness, lineColor, 0)
        {
            Base = typeof(GraphicElement);
        }

        public GraphicEllipse(string properties, byte[] elementBytes)
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
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^GE300,100,10,B ^ FS
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^GE" + context.Scale(Width) + "," + context.Scale(Height) + "," + context.Scale(BorderThickness) + "," + LineColor);
            if (AddFieldSeparator)
                result.Add("^FS");

            return result;
        }
    }
}