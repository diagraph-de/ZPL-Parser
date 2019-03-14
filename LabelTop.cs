using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class LabelTop : BaseElement
    {
        private static LabelTop _current;
        private readonly string properties;
        private byte[] elementBytes;

        public LabelTop(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;
            Current = this;
            Top = Convert.ToInt32(this.properties.Split(',')[0]);
        }

        public LabelTop(int top)
        {
            Top = top;
        }

        public static LabelTop Current
        {
            get { return _current ?? (_current = new LabelTop(0)); }
            set { _current = value; }
        }

        public int Top { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^LT40 in dot rows -120 to 120 
            var result = new List<string>();
            result.Add("^LT" + context.Scale(Top));
            return result;
        }
    }
}