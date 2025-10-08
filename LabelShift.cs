using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class LabelShfit : BaseElement
    {
        private static LabelShfit _current;
        private readonly string properties;
        private byte[] elementBytes;

        public LabelShfit(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;
            Current = this;
            ShiftLeft = Convert.ToInt32(this.properties.Split(',')[0]);
        }

        public LabelShfit(int shiftLeft)
        {
            ShiftLeft = shiftLeft;
        }

        public static LabelShfit Current
        {
            get => _current ?? (_current = new LabelShfit(0));
            set => _current = value;
        }

        public int ShiftLeft { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^LL40 in dots -9999 to 9999 
            var result = new List<string>();
            result.Add("^LS" + context.Scale(ShiftLeft));
            return result;
        }
    }
}