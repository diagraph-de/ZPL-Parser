using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL;

public class BaseFontIdentifier : BaseElement
{
    public BaseFontIdentifier(string fontReplaceLetter, string device, string fontFileName)
    {
        FontReplaceLetter = fontReplaceLetter;
        Device = device;
        FontFileName = fontFileName;
    }

    public string FontReplaceLetter { get; set; }
    public string Device { get; set; }
    public string FontFileName { get; set; }

    public override IEnumerable<string> Render(ZPLRenderOptions context)
    {
        //^CWa,d:o.x 
        var result = new List<string>();
        result.Add("^CW" + FontReplaceLetter + "," + Device + ":" + FontFileName);
        return result;
    }
}