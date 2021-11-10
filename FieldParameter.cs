using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class FieldParameter : FieldElement
    {
        private static FieldParameter _current;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldParameter(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');
            if (sp.Length > 0)
                switch (sp[0].ToUpper())
                {
                    case "H":
                        Direction = Enums.Direction.H; //Horizontal printing (left to right)
                        break;
                    case "R":
                        Direction = Enums.Direction.R; //vertical printing (top to bottom)
                        break;
                    case "I":
                        Direction = Enums.Direction.R; //reverse printing (right to left)
                        break;
                }

            if (sp.Length > 1)
                Gap = Convert.ToInt16(sp[1]);
        }

        public FieldParameter(Enums.Direction direction = Enums.Direction.H, int gap = 0)
        {
            Base = typeof(FieldElement);
            Direction = direction;
            Gap = gap;
        }

        public static FieldParameter Current
        {
            get => _current ?? (_current = new FieldParameter());
            set => _current = value;
        }

        public Enums.Direction Direction { get; set; } = Enums.Direction.H;
        public int Gap { get; set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FP 
            var result = new List<string>();
            result.Add("^FP" + Direction + "," + Gap);
            return result;
        }
    }
}