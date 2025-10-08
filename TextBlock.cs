using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class TextBlock : TextField
    {
        public TextBlock(int positionX, int positionY, string text, int width, int height, ScalableBitmappedFont font,
            NewLineConversionMethod newLineConversion = NewLineConversionMethod.ToSpace,
            bool useHexadecimalIndicator = true)
            : base(positionX, positionY, text, font, newLineConversion, useHexadecimalIndicator)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; }

        public int Height { get; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            if (Font != null)
            {
                result.AddRange(Font.Render(context));
                if (Origin != null)
                    result.AddRange(Origin.Render(context));
                result.Add("^TB" + Font.Orientation + "," + context.Scale(Width) + "," + context.Scale(Height));
            }

            result.Add(RenderFieldDataSection());

            return result;
        }
    }
}