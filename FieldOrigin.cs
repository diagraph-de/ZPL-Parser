using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class FieldOrigin : FieldElement
    {
        private static FieldOrigin _currentPosition;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldOrigin(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            PositionX = Convert.ToInt32(this.properties.Split(',')[0]);
            PositionY = Convert.ToInt32(this.properties.Split(',')[1]);
            HasChild = true;
        }

        public FieldOrigin(int positionX, int positionY)
        {
            Base = typeof(FieldElement);
            PositionX = positionX;
            PositionY = positionY;
        }

        public static FieldOrigin Current
        {
            get { return _currentPosition ?? (_currentPosition = new FieldOrigin(0, 0)); }
            set { _currentPosition = value; }
        }

        public int PositionX { get; protected set; }
        public int PositionY { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FO50,50 
            var result = new List<string>();
            result.Add("^FO" + context.Scale(PositionX) + "," + context.Scale(PositionY));
            return result;
        }
    }
}