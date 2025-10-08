using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class FieldVariable : FieldElement
    {
        private static FieldVariable _currentPosition;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldVariable(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            VariableField = this.properties;
        }

        public FieldVariable(string variableField)
        {
            Base = typeof(FieldElement);
            VariableField = variableField;
        }

        public static FieldVariable Current
        {
            get =>
                _currentPosition != null
                    ? _currentPosition
                    : _currentPosition = new FieldVariable("");
            set => _currentPosition = value;
        }

        public string VariableField { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FVa 
            var result = new List<string>();
            result.Add("^FV" + VariableField);
            return result;
        }
    }
}