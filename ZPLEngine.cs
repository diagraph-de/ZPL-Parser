#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Diagraph.Labelparser.ZPL;

public class ZPLEngine : List<BaseElement>
{
    /// <summary>
    ///     Start an empty engine
    /// </summary>
    public ZPLEngine(string zpl = "")
    {
        if (zpl.Length > 0)
        {
        }
    }

    /// <summary>
    ///     Start an engine with given elements
    /// </summary>
    /// <param name="elements">ZPL elements to be added</param>
    public ZPLEngine(IEnumerable<BaseElement> elements) : base(elements)
    {
    }

    /// <summary>
    ///     Output the ZPL string using given context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public List<string> Render(ZPLRenderOptions context)
    {
        var result = new List<string>
        {
            "^XA",
            context.ChangeInternationalFontEncoding
        };
        foreach (var e in this.Where(x => x.IsEnabled))
        {
            //Empty line
            if (context.AddEmptyLineBeforeElementStart)
                result.Add("");

            //Comments
            if (context.DisplayComments)
                if (e.Comments.Any())
                {
                    result.Add("^FX");
                    e.Comments.ForEach(x => result.Add("//" + x.Replace("^", "[caret]").Replace("~", "[tilde]")));
                }

            //Actual element
            if (context.CompressedRendering)
                result.Add(string.Join("", e.Render(context).ToArray()));
            else
                result.AddRange(e.Render(context));
        }

        result.Add("^XZ");

        return result;
    }

    public string ToZPLString(ZPLRenderOptions context)
    {
        return string.Join("\n", Render(context).ToArray());
    }

    /// <summary>
    ///     Add raw ZPL fragment
    /// </summary>
    /// <param name="rawZPLCode"></param>
    public void AddRawZPLCode(string rawZPLCode)
    {
        Add(new BaseRaw(rawZPLCode));
    }

    /// <summary>
    ///     Convert a char to be Hex value, 2 letters
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string MapToHexadecimalValue(char input)
    {
        return Convert.ToByte(input).ToString("X2");
    }
}