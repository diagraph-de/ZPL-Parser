using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class BarcodeCode39 : Barcode1D
    {
        private static BarcodeCode39 _current;
        private readonly string properties;
        private byte[] elementBytes;

        public BarcodeCode39(int positionX, int positionY, string content, int height = 100,
            Enums.Orientation orientation = Enums.Orientation.N,
            bool printInterpretationLine = true,
            bool printInterpretationLineAboveCode = false, bool mod43CheckDigit = false)
            : base(positionX, positionY, content, height, orientation, printInterpretationLine,
                printInterpretationLineAboveCode)
        {
            Base = typeof(Barcode1D);
            Mod43CheckDigit = mod43CheckDigit;
        }

        public BarcodeCode39(string properties, byte[] elementBytes)
        {
            Base = typeof(Barcode1D);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            RenderProperties = true;
            AddFieldData = false;

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
                default:
                    RenderProperties = false;
                    break;
            }

            Mod43CheckDigit = sp.Length > 1 && sp[1].ToUpper() == "Y";

            if (sp.Length > 2)
                Height = Convert.ToInt32(sp[2]);

            PrintInterpretationLine = sp.Length > 3 && sp[3].ToUpper() == Enums.YesNo.Y.ToString();
            PrintInterpretationLineAboveCode = sp.Length > 4 && sp[4].ToUpper() == Enums.YesNo.Y.ToString();
        }

        public static BarcodeCode39 Current
        {
            get => _current ?? (_current = new BarcodeCode39(0, 0, ""));
            set => _current = value;
        }

        public bool Mod43CheckDigit { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FO100,100 ^ BY3
            //^BCN,100,Y,N,N
            //^FD123456 ^ FS
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^B3" + Orientation + ","
                       + (Mod43CheckDigit
                           ? "Y"
                           : "N") + ","
                       + context.Scale(Height) + ","
                       + (PrintInterpretationLine
                           ? "Y"
                           : "N") + ","
                       + (PrintInterpretationLineAboveCode
                           ? "Y"
                           : "N"));
            if (AddFieldData)
                result.Add("^FD" + Content + "^FS");

            return result;
        }
    }
}