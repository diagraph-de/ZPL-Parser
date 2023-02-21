using System.Collections.Generic;
using System.Text;

namespace Diagraph.Labelparser.ZPL
{
    public class BaseRaw : BaseElement
    {
        public BaseRaw(string rawZPLCode)
        {
            RawContent = Encoding.UTF8.GetBytes(rawZPLCode);
            ;
        }

        public BaseRaw(byte[] rawZPLCode)
        {
            RawContent = rawZPLCode;
        }

        protected BaseRaw()
        {
        }

        public byte[] RawContent { get; set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            result.Add(Encoding.UTF8.GetString(RawContent));
            return result;
        }
    }
}