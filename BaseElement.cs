#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Diagraph.Labelparser.ZPL
{
    public abstract class BaseElement
    {
        public BaseElement()
        {
            Comments = new List<string>();
            IsEnabled = true;
        }

        public List<string> Comments { get; protected set; }

        //Indicate the rendering process whether this elemenet can be skipped
        public bool IsEnabled { get; set; }

        //Optionally identify the element for future lookup/manipulation
        public string Id { get; set; } = Guid.NewGuid().ToString().ToUpper();

        public bool HasChild { get; protected set; } = false;
        public BaseElement Child { get; set; }
        public BaseElement Parent { get; set; }
        public Type Base { get; set; }

        public IEnumerable<string> Render()
        {
            return Render(ZPLRenderOptions.DefaultOptions);
        }

        public string RenderToString()
        {
            return string.Join(" ", Render().ToArray());
        }

        public abstract IEnumerable<string> Render(ZPLRenderOptions context);

        public string ToZPLString()
        {
            return ToZPLString(ZPLRenderOptions.DefaultOptions);
        }

        public string ToZPLString(ZPLRenderOptions context)
        {
            return string.Join("\n", Render(context).ToArray());
        }
    }

    //^A – Scalable/Bitmapped Font

    //^FO – Field Origin

    //^FD – Field Data

    //^FB – Field Block

    //Similar to ZPLTextField with big line spacing, so only the first line is visible

    //The ^TB command prints a text block with defined width and height. The text block has an automatic word-wrap function.If the text exceeds the block height, the text is truncated. Does not support \n
}