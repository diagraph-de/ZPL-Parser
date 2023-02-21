using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class BaseBarCodeFieldDefault : BaseElement
    {
        public BaseBarCodeFieldDefault(int moduleWidth = 2, double barWidthRatio = 3.0d, int height = 10)
        {
            ModuleWidth = moduleWidth;
            BarWidthRatio = barWidthRatio;
            Height = height;
        }

        public int ModuleWidth { get; }

        public double BarWidthRatio { get; }

        public int Height { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            result.Add("^BY" + context.Scale(ModuleWidth) + "," + Math.Round(BarWidthRatio, 1) + "," +
                       context.Scale(Height));
            return result;
        }
    }
}