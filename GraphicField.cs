using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Diagraph.Labelparser.ZPL
{
    public class GraphicField : GraphicElement
    {
        private static GraphicField _current;
        private readonly string properties;

        private Bitmap _bitmap;
        private byte[] elementBytes;

        public GraphicField(string properties, byte[] elementBytes)
        {
            Base = typeof(GraphicElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            //while (this.properties.Contains(",,"))
            //    this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');
            if (sp.Length > 0)
                switch (sp[0].ToUpper())
                {
                    case "A":
                        CompressionType = Enums.CompressionType.A; //Ascii Hexadecimal
                        break;
                    case "B":
                        CompressionType = Enums.CompressionType.B; //binary
                        break;
                    case "C":
                        CompressionType = Enums.CompressionType.C; //compressed binary
                        break;
                }

            if (sp.Length > 1)
                BinaryByteCount = Convert.ToInt32(sp[1]);

            if (sp.Length > 2)
                GraphicFieldCount = Convert.ToInt32(sp[2]);

            if (sp.Length > 3)
                BytesPerRow = Convert.ToInt32(sp[3]);

            if (sp.Length > 4)
            {
                var compressed = false;
                var i = 0;
                var sb = new StringBuilder();

                foreach (var s in sp)
                {
                    if (i > 3)
                    {
                        if (s.Trim() == "")
                        {
                            compressed = true;
                            sb.Append(",");
                        }
                        else
                        {
                            sb.Append(s);
                        }
                    }

                    i++;
                }

                Data = sb.ToString().Trim();

                try
                {
                    //create bitmapfrom base64 png
                    Bitmap = CreateBitmapFromPng(Data);
                }
                catch (Exception e)
                {
                    Bitmap = CreateBitmap(compressed);
                }
            }
        }

        public GraphicField(Enums.CompressionType compressionType, int binaryByteCount, int graphicFieldCount,
            int bytesPerRow, string data)
        {
            Base = typeof(GraphicElement);
            CompressionType = compressionType;
            BinaryByteCount = binaryByteCount;
            GraphicFieldCount = graphicFieldCount;
            BytesPerRow = bytesPerRow;
            Data = data;
            Bitmap = CreateBitmap();
        }

        public static GraphicField Current
        {
            get => _current ?? (_current = new GraphicField(Enums.CompressionType.A, 0, 0, 0, ""));
            set => _current = value;
        }

        public Enums.CompressionType CompressionType { get; } = Enums.CompressionType.A;
        public int BinaryByteCount { get; private set; }
        public int GraphicFieldCount { get; private set; }
        public int BytesPerRow { get; private set; }
        public string Data { get; private set; }

        public Bitmap Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                if (_bitmap != null)
                {
                    if (CompressionType == Enums.CompressionType.A)
                    {
                        var img = ImageHelper.ZPLfromBitmap(_bitmap, false);
                        Data = img.Result;
                        BytesPerRow = img.WidthBytes;
                        BinaryByteCount = img.TotalBytes;
                        GraphicFieldCount = img.TotalBytes;
                    }
                    else
                    {
                        var img = ImageHelper.ZPLfromBitmap(_bitmap, false, true);
                        Data = img.Result;
                        BytesPerRow = img.WidthBytes;
                        BinaryByteCount = img.TotalBytes;
                        GraphicFieldCount = img.TotalBytes;
                    }
                }
                //Data = GetBitmapData();
            }
        }

        public static Bitmap CreateBitmapFromPng(string base64Png)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(base64Png);
                using (var ms = new MemoryStream(imageBytes))
                {
                    var bitmap = new Bitmap(ms);
                    return new Bitmap(bitmap);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding Base64 PNG: {ex.Message}");
                return null;
            }
        }

        private Bitmap CreateBitmap(bool compressed = false)
        {
            var bmp = new Bitmap(1, 1);

            switch (CompressionType)
            {
                case Enums.CompressionType.A:
                    var strucZPL = new ImageHelper.strucZPL();
                    strucZPL.TotalBytes = BinaryByteCount;
                    strucZPL.WidthBytes = BytesPerRow;
                    strucZPL.bytes = Encoding.ASCII.GetBytes(Data);
                    if (Data != "")
                    {
                        try
                        {
                            if (Data.StartsWith(":Z64"))
                            {
                                var imageData = ImageHelper.DecompressZb64(Data.Substring(5));

                                var width = BytesPerRow * 8;
                                var height = imageData.Length / BytesPerRow;
                                strucZPL.TotalBytes = imageData.Length;

                                var bmp1 = ArrayToBitmap(imageData, width, height, PixelFormat.Format1bppIndexed);
                                return bmp1;
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                        bmp = (Bitmap)new ImageHelper().ZPLToBitmap(strucZPL, compressed);
                    }

                    break;
                case Enums.CompressionType.B:
                    break;
                case Enums.CompressionType.C:
                    break;
            }

            return bmp;
        }

        public static Bitmap ArrayToBitmap(byte[] bytes, int width, int height, PixelFormat pixelFormat)
        {
            var image = new Bitmap(width, height, pixelFormat);
            var imageData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite, pixelFormat);
            try
            {
                Marshal.Copy(bytes, 0, imageData.Scan0, bytes.Length);
            }
            finally
            {
                image.UnlockBits(imageData);
            }

            return image;
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^GSo,h,w
            var result = new List<string>();
            result.Add("^GF" + CompressionType + "," + BinaryByteCount + "," + GraphicFieldCount + "," + BytesPerRow +
                       "," + Data);
            return result;
        }
    }
}