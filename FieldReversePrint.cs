using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL;

public class FieldReversePrint : FieldElement
{
    private static FieldReversePrint _current;

    public FieldReversePrint(string properties, byte[] elementBytes)
    {
        Base = typeof(FieldElement);
        HasChild = true;
        _current = this;
    }

    public FieldReversePrint(string data)
    {
        Base = typeof(FieldElement);
        HasChild = true;
    }

    public static FieldReversePrint Current { get; set; }

    public override IEnumerable<string> Render(ZPLRenderOptions context)
    {
        //^FR 
        var result = new List<string>();
        result.Add("^FR");
        return result;
    }
}