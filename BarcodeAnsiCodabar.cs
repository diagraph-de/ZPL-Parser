using System;
using System.Collections.Generic;
using System.Linq;

namespace Allen.Labelparser.ZPL
{
    public class BarcodeAnsiCodabar : Barcode1D
    {
        private static BarcodeAnsiCodabar _current;
        private readonly string properties;
        private byte[] elementBytes;

        public BarcodeAnsiCodabar(int positionX, int positionY, string content, int height, char startCharacter,
            char stopCharacter,
            Enums.Orientation orientation = Enums.Orientation.N, bool printInterpretationLine = true,
            bool printInterpretationLineAboveCode = false,
            bool checkDigit = false)
            : base(positionX, positionY, content, height, orientation, printInterpretationLine,
                printInterpretationLineAboveCode)
        {
            Base = typeof(Barcode1D);
            CheckDigit = checkDigit;
            if (!"ABCDabcd".Contains(startCharacter))
                throw new InvalidOperationException("ANSI Codabar start charactor must be one of A, B, C D");
            StartCharacter = char.ToUpper(startCharacter);
            if (!"ABCDabcd".Contains(stopCharacter))
                throw new InvalidOperationException("ANSI Codabar stop charactor must be one of A, B, C D");
            StopCharacter = char.ToUpper(stopCharacter);
        }

        public BarcodeAnsiCodabar(string properties, byte[] elementBytes)
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

            CheckDigit = sp.Length > 1 && sp[1].ToUpper() == "Y";

            if (sp.Length > 2)
                Height = Convert.ToInt32(sp[2]);

            PrintInterpretationLine = sp.Length > 3 && sp[3].ToUpper() == Enums.YesNo.Y.ToString();
            PrintInterpretationLineAboveCode = sp.Length > 4 && sp[4].ToUpper() == Enums.YesNo.Y.ToString();

            if (sp.Length > 5)
                StartCharacter = sp[3].ToCharArray()[0];

            if (sp.Length > 6)
                StartCharacter = sp[3].ToCharArray()[6];
        }

        public static BarcodeAnsiCodabar Current
        {
            get => _current ?? (_current = new BarcodeAnsiCodabar(0, 0, "", 1, 'A', 'A'));
            set => _current = value;
        }

        public bool CheckDigit { get; }
        public char StartCharacter { get; }
        public char StopCharacter { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FO100,100 ^ BY3
            // ^ BKN,N,150,Y,N,A,A
            //  ^ FD123456 ^ FS
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^BK" + Orientation + ","
                       + (CheckDigit
                           ? "Y"
                           : "N") + ","
                       + context.Scale(Height) + ","
                       + (PrintInterpretationLine
                           ? "Y"
                           : "N") + ","
                       + (PrintInterpretationLineAboveCode
                           ? "Y"
                           : "N") + ","
                       + StartCharacter + ","
                       + StopCharacter);
            if (AddFieldData)
                result.Add("^FD" + Content + "^FS");

            return result;
        }
    }
}