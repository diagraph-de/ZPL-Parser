using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class FieldData : FieldElement
    {
        private static FieldData _current;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldData(BaseElement child, string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            Child = child;
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            var fd = this.properties;

            //replace FieldHexadecimalIndicator
            var indicator = FieldHexadecimalIndicator.Current.Indicator;
            for (var i = 0; i < 255; i++)
            {
                var find = indicator + i.ToString("X4").ToLower();
                var repl = (char)i;
                fd = fd.Replace(find, repl.ToString());
            }

            Data = fd;
        }

        public FieldData(string data)
        {
            Base = typeof(FieldElement);
            Data = data; //data to be printed, max 3072
        }

        public static FieldData Current
        {
            get => _current ?? (_current = new FieldData(""));
            set => _current = value;
        }

        public string Data { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FD123 
            var result = new List<string>();
            result.Add("^FD" + Data);
            return result;
        }
    }
}