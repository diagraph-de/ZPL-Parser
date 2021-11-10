using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class LabelLength : BaseElement
    {
        private static LabelLength _current;
        private readonly string properties;
        private byte[] elementBytes;

        public LabelLength(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            Length = Convert.ToInt32(this.properties.Split(',')[0]);
        }

        public LabelLength(int length)
        {
            Length = length;
        }

        public static LabelLength Current
        {
            get => _current ?? (_current = new LabelLength(0));
            set => _current = value;
        }

        public int Length { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^LL40 in dots 1-32000 
            var result = new List<string>();
            result.Add("^LL" + context.Scale(Length));
            return result;
        }
    }
}