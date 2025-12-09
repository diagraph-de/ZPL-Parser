# 📦 ZPL-Parser

A C# parser and renderer for Zebra Programming Language (ZPL)

This project provides a complete .NET library for parsing, analyzing, rendering, and processing ZPL labels (Zebra Label Format).
It supports common ZPL commands, barcodes, graphics, text objects, font handling, and offers a modular, extensible architecture.

---

## 🚀 Features

- Parsing & tokenizing ZPL label files
- Support for ZPL elements such as:
  - **Text fields** (`^A`, `^FB`, `^FD`, …)
  - **1D/2D barcodes** (`^BC`, `^B3`, `^B7`, `^BQ`, …)
  - **Graphics & recall graphics** (`^GF`, `~DG`)
  - **Format structures** (`^XA`, `^XZ`)
  - **Field positions & origins** (`^FO`, `^FT`)
- Internal engine:
  - `ZplParser` — Converts ZPL commands into structured objects  
  - `ZPLEngine` — Prepares objects for rendering/export
- Rendering support:
  - `ZPLRenderOptions` for DPI, label size, scaling
  - Foundation for bitmap rendering (partial implementation included)
- Clean object-oriented architecture built around `BaseElement`
- Extensible — new ZPL commands can be added easily
- Example code & test files included

---

## 📁 Project Structure

```
ZPL-Parser/
│
├── Barcode1D.cs
├── Barcode2D.cs
├── BarcodeCode128.cs
├── BarcodeCode39.cs
├── BarcodeQR.cs
├── BarcodeDatamatrix.cs
│
├── BaseElement.cs
├── BaseFontIdentifier.cs
├── BaseReferenceGrid.cs
├── BaseRaw.cs
│
├── TextField.cs
├── TextBlock.cs
├── SingleLineFieldBlock.cs
│
├── ScalableBitmappedFont.cs
├── RecallGraphic.cs
├── DownloadGraphic.cs
│
├── ZplParser.cs
├── ZPLEngine.cs
├── ZPLConstants.cs
├── ZPLRenderOptions.cs
│
├── etc/                → Fonts, resources
├── Properties/
│
├── ZPLIIcommandreference.pdf
└── Test.cs (demo / examples)
```

---

## 🔧 Installation

This project is a standalone C#/.NET library.
Simply include it in your existing solution.

### Using .NET CLI

```bash
git clone https://github.com/<user>/ZPL-Parser.git
cd ZPL-Parser
```

To reference it in another project:

```bash
dotnet add reference <path-to-project>
```

---

## 🧩 Example: Parsing ZPL

```csharp
var zpl = "^XA^FO50,50^A0N,50,50^FDHello World^FS^XZ";

var parser = new ZplParser(zpl);
var elements = parser.Parse();

foreach (var el in elements)
{
    Console.WriteLine(el.GetType().Name);
}
```

---

## 🖨️ Example: Preparing a Rendered Output

```csharp
var engine = new ZPLEngine(elements);
var renderOptions = new ZPLRenderOptions
{
    DPI = 203,
    LabelWidth = 800,
    LabelHeight = 600
};

var result = engine.Process(renderOptions);
// Result can be used for rendering
```

---

## 🎯 Target Audience

- Developers working with ZPL in C#/.NET  
- Label design and printing software providers  
- Tools for previewing, analyzing, and validating Zebra labels  
- Systems converting ZPL into images or other formats  

---

## 📚 Documentation

The project includes:

```
ZPLIIcommandreference.pdf
```

This is the official reference for Zebra Programming Language (ZPL II),
useful for extending or validating the implementation.

---

## 🧪 Tests & Examples

The file `Test.cs` demonstrates basic usage of parser and engine.
Sample ZPL input can be modified directly within the project for testing.

---

## 🔮 Planned Enhancements

- Complete bitmap renderer (`System.Drawing`)
- PDF export
- Extended support for nested field blocks
- Full unit test suite
- Publish as NuGet package
