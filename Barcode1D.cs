namespace ZPLParser
{
    public abstract class Barcode1D : PositionedElement
    {
        public Barcode1D(int positionX, int positionY, string content, int height, Enums.Orientation orientation,
            bool printInterpretationLine,
            bool printInterpretationLineAboveCode) : base(positionX, positionY)
        {
            Origin = new FieldOrigin(positionX, positionY);
            Content = content;
            Height = height;
            Orientation = orientation;
            PrintInterpretationLine = printInterpretationLine;
            PrintInterpretationLineAboveCode = printInterpretationLineAboveCode;
            HasChild = true;
        }

        public Barcode1D()
        {
            HasChild = true;
        }

        public bool RenderProperties { get; protected set; } = true;
        public int Height { get; protected set; }
        public Enums.Orientation Orientation { get; protected set; }
        public string Content { get; protected set; }
        public bool PrintInterpretationLine { get; protected set; }
        public bool PrintInterpretationLineAboveCode { get; protected set; }
    }
}