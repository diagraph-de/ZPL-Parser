using System.Collections.Generic;

namespace ZPLParser
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
            return new[] {"^XA"};
        }
    }
}