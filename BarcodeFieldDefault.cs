using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class BarcodeFieldDefault : BaseElement
    {
        private static BarcodeFieldDefault _current;
        private readonly string properties;
        private byte[] elementBytes;

        public BarcodeFieldDefault(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');

            ModuleWidth = Convert.ToInt32(sp[0]);
            Ratio = Convert.ToInt32(sp[1]);
            Height = Convert.ToInt32(sp[2]);
        }

        public BarcodeFieldDefault(int moduleWidth, int ratio, int height)
        {
            ModuleWidth = moduleWidth;
            Ratio = ratio;
            Height = height;
        }

        public static BarcodeFieldDefault Current
        {
            get { return _current ?? (_current = new BarcodeFieldDefault(2, 0, 10)); }
            set { _current = value; }
        }

        public int ModuleWidth { get; protected set; }
        public int Ratio { get; protected set; }
        public int Height { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^BYw,r,h
            var result = new List<string>();
            result.Add("^BY" + ModuleWidth + ',' + Ratio + ',' + Height);
            return result;
        }
    }
}