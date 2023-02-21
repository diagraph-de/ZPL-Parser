using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class FieldSeparator : FieldElement
    {
        public FieldSeparator(BaseElement child, string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            Child = child;
            FieldReversePrint.Current = null;
        }

        public FieldSeparator(string data)
        {
            Base = typeof(FieldElement);
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FS 
            var result = new List<string>();
            result.Add("^FS");
            return result;
        }
    }
}