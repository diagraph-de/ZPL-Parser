using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL;

public class FieldTypeset : FieldElement
{
    private static FieldTypeset _currentPosition;
    private readonly string properties;
    private byte[] elementBytes;

    public FieldTypeset(string properties, byte[] elementBytes)
    {
        Base = typeof(FieldElement);
        this.properties = properties.Replace(Environment.NewLine, "");
        this.elementBytes = elementBytes;

        Current = this;
        PositionX = Convert.ToInt32(this.properties.Split(',')[0]);
        PositionY = Convert.ToInt32(this.properties.Split(',')[1]);
    }

    public FieldTypeset(int positionX, int positionY)
    {
        Base = typeof(FieldElement);
        PositionX = positionX;
        PositionY = positionY;
    }

    public static FieldTypeset Current
    {
        get =>
            _currentPosition != null
                ? _currentPosition
                : _currentPosition = new FieldTypeset(0, 0);
        set => _currentPosition = value;
    }

    public int PositionX { get; protected set; }
    public int PositionY { get; protected set; }

    public override IEnumerable<string> Render(ZPLRenderOptions context)
    {
        //^FT0,0 
        var result = new List<string>();
        result.Add("^FT" + context.Scale(PositionX) + "," + context.Scale(PositionY));
        return result;
    }
}