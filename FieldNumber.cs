using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class FieldNumber : FieldElement
    {
        private static FieldNumber _currentPosition;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldNumber(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            Number = Convert.ToInt32(this.properties.Split(',')[0]);
        }

        public FieldNumber(int number = 0)
        {
            Base = typeof(FieldElement);
            Number = number;
        }

        public static FieldNumber Current
        {
            get => _currentPosition ?? (_currentPosition = new FieldNumber());
            set => _currentPosition = value;
        }

        public int Number { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FN0
            var result = new List<string>();
            result.Add("^FN" + Number);
            return result;
        }
    }
}