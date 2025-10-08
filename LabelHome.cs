using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class LabelHome : BaseElement
    {
        private static LabelHome _currentPosition;
        private readonly string properties;
        private byte[] elementBytes;

        public LabelHome(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            PositionX = Convert.ToInt32(this.properties.Split(',')[0]);
            PositionY = Convert.ToInt32(this.properties.Split(',')[1]);
        }

        public LabelHome(int positionX, int positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }

        public static LabelHome Current
        {
            get => _currentPosition ?? (_currentPosition = new LabelHome(0, 0));
            set => _currentPosition = value;
        }

        public int PositionX { get; protected set; }
        public int PositionY { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^LH0,0 
            var result = new List<string>();
            result.Add("^LH" + context.Scale(PositionX) + "," + context.Scale(PositionY));
            return result;
        }
    }
}