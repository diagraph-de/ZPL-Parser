using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class PrintWidth : BaseElement
    {
        private static PrintWidth _current;
        private readonly string properties;
        private byte[] elementBytes;

        public PrintWidth(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;
            Current = this;
            Width = Convert.ToInt32(this.properties.Split(',')[0]);
        }

        public PrintWidth(int width)
        {
            Width = width;
        }

        public static PrintWidth Current
        {
            get { return _current ?? (_current = new PrintWidth(0)); }
            set { _current = value; }
        }

        public int Width { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^PW609 label width in dots 2 to width 
            var result = new List<string>();
            result.Add("^PW" + context.Scale(Width));
            return result;
        }
    }
}