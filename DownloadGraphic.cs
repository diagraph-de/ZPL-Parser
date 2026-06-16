using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Diagraph.Labelparser.ZPL;

public class DownloadGraphic : BaseElement
{
    private static DownloadGraphic _current;
    private readonly string properties;
    private byte[] elementBytes;

    public DownloadGraphic(string properties, byte[] elementBytes, bool compressed = false)
    {
        this.properties = properties.Replace(Environment.NewLine, "");
        this.elementBytes = elementBytes;

        Current = this;
        while (this.properties.Contains(",,"))
            this.properties = this.properties.Replace(",,", ",0,");

        var sp = this.properties.Split(',');

        var fileName = sp[0].Split(':')[1];
        var extensionIndex = fileName.LastIndexOf('.');
        DestinationDevice = sp[0].Split(':')[0] + ":";
        ImageName = extensionIndex >= 0 ? fileName.Substring(0, extensionIndex) : fileName;
        FileNameExtension = extensionIndex >= 0 ? NormalizeExtension(fileName.Substring(extensionIndex)) : string.Empty;
        TotalNumberOfBytes = Convert.ToInt16(sp[1]);
        NumberOfRows = Convert.ToInt16(sp[2]);
        var data = sp.Length > 3 ? string.Join(",", sp.Skip(3)) : string.Empty;
        Data = Encoding.ASCII.GetBytes(data);

        if (!string.IsNullOrWhiteSpace(data) && NumberOfRows > 0)
        {
            try
            {
                Image = new ImageHelper().BinaryToBitmap(Data, NumberOfRows * 8, TotalNumberOfBytes / NumberOfRows, false);
            }
            catch
            {
                Image = null;
            }
        }

        //try
        //{
        //    var bmp = new Bitmap(@"C:\Users\daniel\Desktop\label.bmp");
        //    var bmpString = new ImageHelper().BitmapToString(bmp, compressed);

        //    Image = new ImageHelper().BinaryToBitmap(Data, NumberOfRows * 8, TotalNumberOfBytes / NumberOfRows / 8, true);
        //    Image.Save(@"C:\Users\daniel\Desktop\zpl.bmp");
        //}
        //catch (Exception ex)
        //{
        //}
    }

    public DownloadGraphic(string destinationDevice, string imageName, string fileNameExtension,
        int totalNumberOfBytes, int numberOfRows, string data, bool compressed = false)
    {
        DestinationDevice = destinationDevice;
        ImageName = imageName;
        FileNameExtension = NormalizeExtension(fileNameExtension);
        TotalNumberOfBytes = totalNumberOfBytes;
        NumberOfRows = numberOfRows;
        Data = Encoding.ASCII.GetBytes(data);

        Image = new ImageHelper().BinaryToBitmap(Data, numberOfRows * 8, totalNumberOfBytes / numberOfRows, compressed);
    }

    public static DownloadGraphic Current
    {
        get => _current ?? (_current = new DownloadGraphic("R:", "image", ".GRF", 0, 0, ""));
        set => _current = value;
    }

    public string DestinationDevice { get; protected set; }
    public string ImageName { get; protected set; }
    public string FileNameExtension { get; protected set; }
    public int TotalNumberOfBytes { get; protected set; }
    public int NumberOfRows { get; protected set; }
    public byte[] Data { get; protected set; }
    public Image Image { get; protected set; }

    public override IEnumerable<string> Render(ZPLRenderOptions context)
    {
        //^LL40 in dots -9999 to 9999 
        var result = new List<string>
        {
            "~DG" + DestinationDevice + ImageName + FileNameExtension + "," + TotalNumberOfBytes + "," +
            NumberOfRows + "," + Data
        };
        return result;
    }

    private static string NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        return extension.StartsWith(".") ? extension : "." + extension.TrimStart('.');
    }
}
