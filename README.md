# ZPL-Parser

`ZPL-Parser` is a C# library for parsing and rebuilding Zebra Programming Language labels.
The repository also includes a WinForms demo that exercises the parser and shows a live preview on the right side while you edit ZPL on the left.
Barcode rendering is handled fully locally through the bundled Zint-based implementation in the core project.

## What is included

- `Labelparser-ZPL.csproj` — core parser, ZPL object model, and preview renderer helper
- `Demo/ZPLParser.Demo.WinForms.csproj` — WinForms demo that consumes the core library
- `Zint/` — local barcode engine wrapper plus native runtime files copied to the output folder
- `ZPL-Parser.sln` — solution containing both projects
- `ZPLIIcommandreference.pdf` — Zebra command reference shipped with the repo

## Demo app

The demo UI shows:

- live ZPL input on the left
- rendered preview on the right
- normalized ZPL output
- parser tree
- parsed element list
- preview zoom and fit controls
- quick sample loading and file opening

It includes sample buttons for:

- a mixed label with text, boxes, barcodes, and graphics
- a graphics-focused sample with `^GF`, `~DG`, and `^XG`
- a barcode-focused sample
- the reference shipping label used for pixel checks against the local renderer

The preview renderer lives in the core project and renders labels locally so the demo can show a pixel-matched label preview.
Barcode rendering now uses a real barcode engine in the core project, including GS1-aware Code 128, QR, Micro QR, and Data Matrix support.

## Current parser coverage

The parser currently recognizes the commands implemented in the codebase, including:

- label framing and layout commands such as `^XA`, `^XZ`, `^PW`, `^LL`, `^LH`, `^LT`, `^LS`, `^LR`, `^MM`, `^PQ`
- text and field commands such as `^FO`, `^FT`, `^FD`, `^FS`, `^FB`, `^TB`, `^FH`, `^FR`, `^FV`, `^FP`, `^FC`, `^FN`
- fonts and international encoding helpers such as `^A*`, `^CF`, `^CI`
- graphics and image commands such as `^GB`, `^GC`, `^GD`, `^GE`, `^GF`, `~DG`, `^XG`, `^GS`
- barcodes such as `^B3`, `^BC`, `^BK`, `^BQ`, `^BX`, `^BY`
- comments via `^FX`

The demo preview renders the supported commands visually and keeps the preview canvas at the real label scale so the label does not collapse into a tiny thumbnail.

## Build

Open `ZPL-Parser.sln` in Visual Studio.  
For command-line builds, use the project files directly:

```powershell
dotnet build Labelparser-ZPL.csproj -c Debug
dotnet build Demo/ZPLParser.Demo.WinForms.csproj -c Debug
```

If you want the demo as the startup app, set `ZPLParser.Demo.WinForms` as the startup project.

## Run the demo

```powershell
dotnet run --project Demo/ZPLParser.Demo.WinForms.csproj
```

In the preview tab you can switch between fit-to-window and fixed zoom levels.
The preview is local and driven by the same parser data the demo displays in the tree and element tabs.

## Example usage

```csharp
using Diagraph.Labelparser.ZPL;

var parser = new ZplParser(System.Text.Encoding.UTF8.GetBytes("^XA^FO50,50^A0N,40,40^FDHello^FS^XZ"));
var elements = parser.Elements;

var normalized = new ZPLEngine(elements).ToZPLString(new ZPLRenderOptions
{
    DisplayComments = true
});
```

## Notes

- The core library targets .NET Framework 4.8.
- The WinForms demo is built as x86 so the bundled native Zint barcode engine loads reliably.
- `Test.cs` contains example snippets rather than an automated test suite.
- The solution builds successfully with the demo project and the renderer helper now lives in the core library.
- Native Zint files are copied into `bin\AnyCPU\<Configuration>\zint\` during build.
