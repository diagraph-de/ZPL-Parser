using System;
using System.Collections.Generic;
using System.Drawing;

namespace Diagraph.Labelparser.ZPL;

//[TestClass]
public class BasicTests
{
    //[TestMethod]
    public void SingelElement()
    {
        var result = new GraphicBox(100, 100, 100, 100).ToZPLString();
        Console.WriteLine(result);
    }

    //[TestMethod]
    public void MultipleElements()
    {
        var sampleText = "[_~^][LineBreak\n][The quick fox jumps over the lazy dog.]";
        var font = new ScalableBitmappedFont(50, 50);
        var labelElements = new List<BaseElement>
        {
            new TextField(50, 100, sampleText, font),
            new GraphicBox(400, 700, 100, 100, 5),
            new GraphicBox(450, 750, 100, 100, 50),
            new GraphicCircle(400, 700, 100, 5),
            new GraphicDiagonalLine(400, 700, 100, 50, 5),
            new GraphicDiagonalLine(400, 700, 50, 100, 5),
            new GraphicSymbol(Enums.GraphicSymbolCharacter.Copyright, 600, 600, 50, 50),
            new BarcodeQR(200, 800, "MM,AAC-42"),
            new BaseRaw("^FO200, 200^GB300, 200, 10 ^FS")
        };

        //Add raw ZPL code

        var renderEngine = new ZPLEngine(labelElements);
        var output = renderEngine.ToZPLString(new ZPLRenderOptions { AddEmptyLineBeforeElementStart = true });

        Console.WriteLine(output);
    }

    //[TestMethod]
    public void ChangeDPI()
    {
        var labelElements = new List<BaseElement> { new GraphicBox(400, 700, 100, 100, 5) };

        var options = new ZPLRenderOptions { SourcePrintDPI = 203, TargetPrintDPI = 300 };
        var output = new ZPLEngine(labelElements).ToZPLString(options);

        Console.WriteLine(output);
    }

    //[TestMethod]
    public void RenderComments()
    {
        var labelElements = new List<BaseElement>();

        var textField = new TextField(50, 100, "AAA", ZPLConstants.Font.Default);
        textField.Comments.Add("A important field");
        labelElements.Add(textField);

        var renderEngine = new ZPLEngine(labelElements);
        var output = renderEngine.ToZPLString(new ZPLRenderOptions { DisplayComments = true });

        Console.WriteLine(output);
    }

    //[TestMethod]
    public void TextFieldVariations()
    {
        var sampleText = "[_~^][LineBreak\n][The quick fox jumps over the lazy dog.]";
        var font = new ScalableBitmappedFont(50, 50);

        var labelElements = new List<BaseElement>
        {
            new TextField(10, 10, sampleText, font, useHexadecimalIndicator: false),
            new TextField(10, 50, sampleText, font, useHexadecimalIndicator: true),
            new SingleLineFieldBlock(10, 150, sampleText, 500, font),
            new FieldBlock(10, 300, sampleText, 400, font, 2),
            new TextBlock(10, 600, sampleText, 400, 100, font)
        };
        //Specail character is repalced with space
        //Specail character is using Hex value ^FH
        //Only the first line is displayed
        //Max 2 lines, text exceeding the maximum number of lines overwrites the last line.
        // Multi - line text within a box region

        var renderEngine = new ZPLEngine(labelElements);
        var output = renderEngine.ToZPLString(new ZPLRenderOptions { AddEmptyLineBeforeElementStart = true });

        Console.WriteLine(output);
    }

    //[TestMethod]
    public void DownloadGraphics()
    {
        var labelElements = new List<BaseElement>
        {
            new BaseDownloadGraphics('R', "SAMPLE", ".GRC", new Bitmap("Sample.bmp")),
            new RecallGraphic(100, 100, 'R', "SAMPLE", ".GRC")
        };

        var renderEngine = new ZPLEngine(labelElements);
        var output = renderEngine.ToZPLString(new ZPLRenderOptions
            { AddEmptyLineBeforeElementStart = true, TargetPrintDPI = 600, SourcePrintDPI = 200 });

        Console.WriteLine(output);
    }
}