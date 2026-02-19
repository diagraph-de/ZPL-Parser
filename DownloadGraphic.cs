using System;
using System.Collections.Generic;
using System.Drawing;
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

        DestinationDevice = sp[0].Split(':')[0] + ":";
        ImageName = sp[0].Split(':')[1].Split('.')[0];
        FileNameExtension = "." + sp[0].Split(':')[1].Split('.')[1];
        TotalNumberOfBytes = Convert.ToInt16(sp[1]);
        NumberOfRows = Convert.ToInt16(sp[2]);
        Data = Encoding.ASCII.GetBytes(sp[3]);

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
        FileNameExtension = fileNameExtension;
        TotalNumberOfBytes = totalNumberOfBytes;
        NumberOfRows = numberOfRows;
        Data = Encoding.ASCII.GetBytes(data);

        Image = new ImageHelper().BinaryToBitmap(Data, numberOfRows, totalNumberOfBytes / numberOfRows, compressed);
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
}