using System.Collections.Generic;

namespace Allen.Labelparser.ZPL
{
    public enum NewLineConversionMethod
    {
        ToSpace,
        ToEmpty,
        ToZPLNewLine
    }

    public abstract class PositionedElement : BaseElement
    {
        protected PositionedElement(int positionX, int positionY)
        {
            Origin = new FieldOrigin(positionX, positionY);
        }

        protected PositionedElement()
        {
        }

        public bool AddFieldData { get; protected set; } = true;
        public FieldOrigin Origin { get; protected set; }

        public List<FieldElement> FieldElements { get; set; }
    }
}