#region

using System;
using System.Collections.Generic;

#endregion

namespace ZPLParser
{
    public class GraphicBox : GraphicElement
    {
        private static GraphicBox _current;
        private readonly string properties;

        private byte[] elementBytes;

        public GraphicBox(int positionX, int positionY, int width, int height, int borderThickness = 1, Enums.BlackWhite lineColor = Enums.BlackWhite.B,
            int cornerRounding = 0) :
            base(positionX, positionY, borderThickness, lineColor)
        {
            Base = typeof(GraphicElement);
            Width = width;
            Height = height;

            CornerRounding = cornerRounding;
        }

        public GraphicBox(string properties, byte[] elementBytes)
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
                LineColor = sp[3].ToUpper() == Enums.BlackWhite.B.ToString()
                    ? Enums.BlackWhite.B
                    : Enums.BlackWhite.W;

            if (sp.Length > 4)
                CornerRounding = Convert.ToInt32(sp[4]);
        }

        protected GraphicBox()
        {
        }

        public static GraphicBox Current
        {
            get { return _current ?? (_current = new GraphicBox(0, 0, 0, 0)); }
            set { _current = value; }
        }

        public int Width { get; set; }
        public int Height { get; set; }

        //0~8
        public int CornerRounding { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FO50,50
            //^GB300,200,10 
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^GB" + context.Scale(Width) + "," + context.Scale(Height) + "," + context.Scale(BorderThickness) + "," + LineColor + "," +
                       context.Scale(CornerRounding));
            if (AddFieldSeparator)
                result.Add("^FS");
            return result;
        }
    }
}