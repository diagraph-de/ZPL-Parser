using System;
using System.Collections.Generic;
using System.Text;

namespace Diagraph.Labelparser.ZPL
{
    public class TextField : PositionedElement
    {
        /// <summary>
        ///     Constuct a ^FD (Field Data) element, together with the ^FO, ^A and ^FH.Control character will be handled (Conver to
        ///     Hex or replace with ' ')
        /// </summary>
        /// <param name="text">Original text content</param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="fontWidth"></param>
        /// <param name="fontHeight"></param>
        /// <param name="fontName"></param>
        /// <param name="orientation"></param>
        /// <param name="useHexadecimalIndicator"></param>
        public TextField(int positionX, int positionY, string text, ScalableBitmappedFont font,
            NewLineConversionMethod newLineConversion = NewLineConversionMethod.ToSpace,
            bool useHexadecimalIndicator = true, bool reversePrint = false) :
            base(positionX, positionY)
        {
            Text = text;
            Origin = new FieldOrigin(positionX, positionY);
            Font = font;
            UseHexadecimalIndicator = useHexadecimalIndicator;
            NewLineConversion = newLineConversion;
            ReversePrint = reversePrint;

            HasChild = true;
        }

        protected TextField()
        {
            HasChild = true;
        }

        //^A
        public ScalableBitmappedFont Font { get; protected set; }

        //^FH
        public bool UseHexadecimalIndicator { get; protected set; }

        //^FR
        public bool ReversePrint { get; protected set; }

        public NewLineConversionMethod NewLineConversion { get; protected set; }

        //^FD
        public string Text { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            if (Font != null)
                result.AddRange(Font.Render(context));
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add(RenderFieldDataSection());

            return result;
        }

        protected string RenderFieldDataSection()
        {
            var sb = new StringBuilder();
            sb.Append(UseHexadecimalIndicator
                ? "^FH"
                : "");
            sb.Append(ReversePrint
                ? "^FR"
                : "");
            sb.Append("^FD");
            foreach (var c in Text+"")
                sb.Append(SanitizeCharacter(c));
            sb.Append("^FS");

            return sb.ToString();
        }

        private string SanitizeCharacter(char input)
        {
            if (UseHexadecimalIndicator)
                switch (input)
                {
                    case '_':
                    case '^':
                    case '~':
                        return "_" + Convert.ToByte(input).ToString("X2");
                    case '\\':
                        return " ";
                }
            else
                switch (input)
                {
                    case '^':
                    case '~':
                    case '\\':
                        return " ";
                }

            if (input == '\n')
                switch (NewLineConversion)
                {
                    case NewLineConversionMethod.ToEmpty:
                        return "";
                    case NewLineConversionMethod.ToSpace:
                        return " ";
                    case NewLineConversionMethod.ToZPLNewLine:
                        return @"\&";
                }

            return input.ToString();
        }
    }
}