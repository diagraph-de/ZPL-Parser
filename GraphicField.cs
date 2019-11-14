using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ZPLParser
{
    public class GraphicField : GraphicElement
    {
        private static GraphicField _current;
        private readonly string properties;
        private byte[] elementBytes;

        public GraphicField(string properties, byte[] elementBytes)
        {
            Base = typeof(GraphicElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

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
                bool compressed = false;
                int i = 0;
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
                            sb.Append(s);
                    }
                    i++;
                }

                Data = sb.ToString().Trim();
                Bitmap = CreateBitmap(compressed);
            }
        }

        public GraphicField(Enums.CompressionType compressionType, int binaryByteCount, int graphicFieldCount, int bytesPerRow, string data)
        {
            Base = typeof(GraphicElement);
            CompressionType = compressionType;
            BinaryByteCount = binaryByteCount;
            GraphicFieldCount = graphicFieldCount;
            BytesPerRow = bytesPerRow;
            Data = data;
            Bitmap = CreateBitmap();
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
                        bmp = (Bitmap)new ImageHelper().ZPLToBitmap(strucZPL, compressed);
                    break;
                case Enums.CompressionType.B:
                    break;
                case Enums.CompressionType.C:
                    break;
            }
            return bmp;
        }

        public static GraphicField Current
        {
            get { return _current ?? (_current = new GraphicField(Enums.CompressionType.A, 0, 0, 0, "")); }
            set { _current = value; }
        }

        public Enums.CompressionType CompressionType { get; } = Enums.CompressionType.A;
        public int BinaryByteCount { get; private set; }
        public int GraphicFieldCount { get; private set; }
        public int BytesPerRow { get; private set; }
        public string Data { get; private set; }

        private Bitmap _bitmap;

        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                if (_bitmap != null)
                {
                    if (CompressionType == Enums.CompressionType.A)
                    {
                        var img = ImageHelper.ZPLfromBitmap(_bitmap, false, false);
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

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^GSo,h,w
            var result = new List<string>();
            result.Add("^GF" + CompressionType + "," + BinaryByteCount + "," + GraphicFieldCount + "," + BytesPerRow + "," + Data);
            return result;
        }

    }
}
