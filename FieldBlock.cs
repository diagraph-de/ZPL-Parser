using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class FieldBlock : TextField
    {
        private static FieldBlock _current;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldBlock(int positionX, int positionY, string text, int width, ScalableBitmappedFont font, int maxNumberOfLines = 1, int lineSpace = 0,
            Enums.TextJustification textJustification = Enums.TextJustification.L, int hangingIndent = 0,
            NewLineConversionMethod newLineConversion = NewLineConversionMethod.ToZPLNewLine,
            bool useHexadecimalIndicator = true, bool reversePrint = false)
            : base(positionX, positionY, text, font, newLineConversion, useHexadecimalIndicator, reversePrint)
        {
            Base = typeof(FieldElement);
            TextJustification = textJustification;
            Width = width;
            MaxNumberOfLines = maxNumberOfLines;
            LineSpace = lineSpace;
            HangingIndent = hangingIndent;
        }

        public FieldBlock(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this; 
            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');

            if (sp.Length > 0)
                Width = Convert.ToInt32(sp[0]);

            if (sp.Length > 1)
                MaxNumberOfLines = Convert.ToInt32(sp[1]);

            if (sp.Length > 2)
                LineSpace = Convert.ToInt32(sp[2]);

            if (sp.Length > 3)
            {
                if (sp[3].ToUpper() == Enums.TextJustification.R.ToString())
                    TextJustification = Enums.TextJustification.R;
                if (sp[3].ToUpper() == Enums.TextJustification.L.ToString())
                    TextJustification = Enums.TextJustification.L;
                if (sp[3].ToUpper() == Enums.TextJustification.J.ToString())
                    TextJustification = Enums.TextJustification.J;
                if (sp[3].ToUpper() == Enums.TextJustification.C.ToString())
                    TextJustification = Enums.TextJustification.C;
            }

            if (sp.Length > 4)
                HangingIndent = Convert.ToInt32(sp[4]);
        }

        public static FieldBlock Current
        {
            get { return _current ?? (_current = new FieldBlock(0, 0, "", 0, new ScalableBitmappedFont(50, 50))); }
            set { _current = value; }
        }

        public int Width { get; }

        public int MaxNumberOfLines { get; } = 1;

        public int LineSpace { get; }

        public Enums.TextJustification TextJustification { get; } = Enums.TextJustification.L;

        //hanging indent (in dots) of the second and remaining lines
        public int HangingIndent { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^XA
            //^CF0,30,30 ^ FO25,50
            //   ^ FB250,4,,
            //^FDFD command that IS\&
            // preceded by an FB \&command.
            // ^ FS
            // ^ XZ

            var result = new List<string>();
            if (Font != null)
                result.AddRange(Font.Render(context));
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^FB" + context.Scale(Width) + "," + MaxNumberOfLines + "," + context.Scale(LineSpace) + "," + TextJustification + "," +
                       context.Scale(HangingIndent));
            result.Add(RenderFieldDataSection());

            return result;
        }
    }
}