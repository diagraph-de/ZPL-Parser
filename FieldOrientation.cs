using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class FieldOrientation : FieldElement
    {
        private static FieldOrientation _current;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldOrientation(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            var sp = this.properties.Split(',');
            if (sp.Length > 0)
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
        }

        public FieldOrientation(Enums.Orientation orientation = Enums.Orientation.N)
        {
            Base = typeof(FieldElement);
            Orientation = orientation; //data to be printed, max 3072
        }

        public static FieldOrientation Current
        {
            get { return _current ?? (_current = new FieldOrientation()); }
            set { _current = value; }
        }

        public Enums.Orientation Orientation { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FWr
            var result = new List<string>();
            result.Add("^FW" + Orientation);
            return result;
        }
    }
}