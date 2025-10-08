using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class BaseReferenceGrid : BaseElement
    {
        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            for (var i = 0; i <= 1500; i += 100)
                result.AddRange(new GraphicBox(0, i, 3000, 1).Render());
            for (var i = 0; i <= 1500; i += 100)
                result.AddRange(new GraphicBox(i, 0, 1, 3000).Render());
            return result;
        }
    }
}