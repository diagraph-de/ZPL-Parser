using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class EndFormat : BaseElement
    {
        public EndFormat(string properties, byte[] elementBytes)
        {
        }

        public EndFormat(string data)
        {
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^XZ
            return new[] { "^XZ" };
        }
    }
}