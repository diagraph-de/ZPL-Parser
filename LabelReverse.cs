using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL;

public class LabelReverse : BaseElement
{
    private static LabelReverse _current;
    private readonly string properties;
    private byte[] elementBytes;

    public LabelReverse(string properties, byte[] elementBytes)
    {
        this.properties = properties.Replace(Environment.NewLine, "");
        this.elementBytes = elementBytes;

        Current = this;
        var r = this.properties.Split(',')[0];
        Reverse = r.ToUpper() == Enums.YesNo.Y.ToString()
            ? Enums.YesNo.Y
            : Enums.YesNo.N;
    }

    public LabelReverse(Enums.YesNo reverse)
    {
        Reverse = reverse;
    }

    public static LabelReverse Current
    {
        get => _current ?? (_current = new LabelReverse(Enums.YesNo.N));
        set => _current = value;
    }

    public Enums.YesNo Reverse { get; protected set; }

    public override IEnumerable<string> Render(ZPLRenderOptions context)
    {
        //^LL40 in dots -9999 to 9999 
        var result = new List<string>();
        result.Add("^LR" + Reverse);
        return result;
    }
}