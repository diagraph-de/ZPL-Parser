using System.Collections.Generic;

namespace ZPLParser
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