namespace Allen.Labelparser.ZPL
{
    public abstract class GraphicElement : PositionedElement
    {
        public GraphicElement(int positionX, int positionY, int borderThickness = 1,
            Enums.BlackWhite lineColor = Enums.BlackWhite.B) :
            base(positionX, positionY)
        {
            BorderThickness = borderThickness;
            LineColor = lineColor;
        }

        protected GraphicElement()
        {
        }

        //Line color
        public Enums.BlackWhite LineColor { get; protected set; }

        public int BorderThickness { get; protected set; }
        public bool AddFieldSeparator { get; protected set; } = true;
    }
}