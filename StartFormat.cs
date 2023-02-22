using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class StartFormat : BaseElement
    {
        public StartFormat(string properties, byte[] elementBytes)
        {
        }

        public StartFormat(string data)
        {
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^XA
            return new[] { "^XA" };
        }
    }
}