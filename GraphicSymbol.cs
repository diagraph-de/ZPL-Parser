using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class GraphicSymbol : PositionedElement
    {
        private static GraphicSymbol _current;
        private readonly string properties;

        private byte[] elementBytes;

        public GraphicSymbol(string properties, byte[] elementBytes)
        {
            Base = typeof(GraphicElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            AddFieldData = false;

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
                    default:
                        //RenderProperties = false;
                        break;
                }

            if (sp.Length > 1)
                Height = Convert.ToInt32(sp[1]);

            if (sp.Length > 2)
                Width = Convert.ToInt32(sp[2]);
        }

        public GraphicSymbol(Enums.GraphicSymbolCharacter character, int positionX, int positionY, int width,
            int height, Enums.Orientation orientation = Enums.Orientation.N) :
            base(positionX, positionY)
        {
            Base = typeof(GraphicElement);
            Character = character;
            Orientation = orientation;
            Width = width;
            Height = height;
        }

        public static GraphicSymbol Current
        {
            get { return _current ?? (_current = new GraphicSymbol(Enums.GraphicSymbolCharacter.Copyright, 0, 0, 0, 0, Enums.Orientation.N)); }
            set { _current = value; }
        }

        public Enums.Orientation Orientation { get; }
        public int Width { get; }
        public int Height { get; }

        public Enums.GraphicSymbolCharacter Character { get; }

        private string CharacterLetter
        {
            get
            {
                switch (Character)
                {
                    case Enums.GraphicSymbolCharacter.RegisteredTradeMark:
                        return "A";
                    case Enums.GraphicSymbolCharacter.Copyright:
                        return "B";
                    case Enums.GraphicSymbolCharacter.TradeMark:
                        return "C";
                    case Enums.GraphicSymbolCharacter.UnderwritersLaboratoriesApproval:
                        return "D";
                    case Enums.GraphicSymbolCharacter.CanadianStandardsAssociationApproval:
                        return "E";
                    default:
                        return "";
                }
            }
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^GSo,h,w
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^GS" + Orientation + "," + context.Scale(Height) + "," + context.Scale(Width));
            if (AddFieldData)
                result.Add("^FD" + CharacterLetter + "^FS");
            return result;
        }
    }
}