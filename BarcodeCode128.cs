using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class BarcodeCode128 : Barcode1D
    {
        private static BarcodeCode128 _current;
        private readonly string properties;
        private byte[] elementBytes;

        public BarcodeCode128(int positionX, int positionY,
            string content, int height = 100,
            Enums.Orientation orientation = Enums.Orientation.N,
            bool printInterpretationLine = true,
            bool printInterpretationLineAboveCode = false)
            : base(positionX, positionY, content, height, orientation, printInterpretationLine, printInterpretationLineAboveCode)
        {
            Base = typeof(Barcode1D);
        }

        public BarcodeCode128(string properties, byte[] elementBytes)
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
            if (sp.Length > 1)
                Height = Convert.ToInt32(sp[1]);
            PrintInterpretationLine = sp.Length > 2 && sp[2].ToUpper() == Enums.YesNo.Y.ToString();
            PrintInterpretationLineAboveCode = sp.Length > 3 && sp[3].ToUpper() == Enums.YesNo.Y.ToString();

            UCCCheckDigit = sp.Length > 4 && sp[4].ToUpper() == "Y"
                ? Enums.YesNo.Y
                : Enums.YesNo.N;

            if (sp.Length > 5)
                switch (sp[5].ToUpper())
                {
                    case "N":
                        Mode = Enums.Mode.N;
                        break;
                    case "U":
                        Mode = Enums.Mode.U;
                        break;
                    case "A":
                        Mode = Enums.Mode.A;
                        break;
                }
        }

        public static BarcodeCode128 Current
        {
            get { return _current ?? (_current = new BarcodeCode128(0, 0, "")); }
            set { _current = value; }
        }

        public Enums.YesNo UCCCheckDigit { get; protected set; } = Enums.YesNo.N;
        public Enums.Mode Mode { get; protected set; } = Enums.Mode.N;

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FO100,100 ^ BY3
            //^BCN,100,Y,N,N
            //^FD123456 ^ FS
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^BC");
            if (RenderProperties)
                result.Add(Orientation + ","
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