namespace Diagraph.Labelparser.ZPL
{
    public class SingleLineFieldBlock : FieldBlock
    {
        public SingleLineFieldBlock(int positionX, int positionY, string text, int width, ScalableBitmappedFont font,
            Enums.TextJustification textJustification = Enums.TextJustification.L,
            NewLineConversionMethod newLineConversion = NewLineConversionMethod.ToSpace,
            bool useHexadecimalIndicator = true, bool reversePrint = false)
            : base(positionX, positionY, text, width, font, 9999, 9999, textJustification, 0, newLineConversion,
                useHexadecimalIndicator, reversePrint)
        {
        }
    }
}