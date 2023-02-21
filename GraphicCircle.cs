using System;
using System.Collections.Generic;

namespace Allen.Labelparser.ZPL
{
    public class GraphicCircle : GraphicElement
    {
        private static GraphicCircle _current;
        private readonly string properties;

        private byte[] elementBytes;

        public GraphicCircle(int positionX, int positionY, int diameter, int borderThickness = 1,
            Enums.BlackWhite lineColor = Enums.BlackWhite.B) :
            base(positionX, positionY,
                borderThickness, lineColor)
        {
            Base = typeof(GraphicElement);
            Diameter = diameter;
        }

        public GraphicCircle(string properties, byte[] elementBytes)
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
                Diameter = Convert.ToInt32(sp[0]);

            if (sp.Length > 1)
                BorderThickness = Convert.ToInt32(sp[1]);

            if (sp.Length > 2)
                LineColor = sp[2].ToUpper() == Enums.BlackWhite.W.ToString()
                    ? Enums.BlackWhite.W
                    : Enums.BlackWhite.B;
        }

        public static GraphicCircle Current
        {
            get => _current ?? (_current = new GraphicCircle(0, 0, 0));
            set => _current = value;
        }

        public int Diameter { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^GCd,t,c
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^GC" + context.Scale(Diameter) + "," + context.Scale(BorderThickness) + "," + LineColor);
            if (AddFieldSeparator)
                result.Add("^FS");
            return result;
        }
    }
}