using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL;

public class FieldHexadecimalIndicator : FieldElement
{
    private static FieldHexadecimalIndicator _current;
    private readonly string properties;
    private byte[] elementBytes;

    public FieldHexadecimalIndicator(string properties, byte[] elementBytes)
    {
        Base = typeof(FieldElement);
        this.properties = properties.Replace(Environment.NewLine, "");
        this.elementBytes = elementBytes;

        Current = this;
        Indicator = Convert.ToChar(this.properties.Split(',')[0]);
    }

    public FieldHexadecimalIndicator(char indicator)
    {
        Base = typeof(FieldElement);
        Indicator = indicator;
    }

    public static FieldHexadecimalIndicator Current
    {
        get => _current ?? (_current = new FieldHexadecimalIndicator('_'));
        set => _current = value;
    }

    public char Indicator { get; protected set; }

    public override IEnumerable<string> Render(ZPLRenderOptions context)
    {
        //^FH\ 
        var result = new List<string>();
        result.Add("^FH" + Indicator);
        return result;
    }
}