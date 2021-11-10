using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class BarcodeDatamatrix : Barcode2D
    {
        private static BarcodeDatamatrix _current;
        private readonly string properties;
        private byte[] elementBytes;

        public BarcodeDatamatrix(
            int positionX,
            int positionY,
            string content,
            string orientation = "N",
            int height = 2,
            int quality = 0,
            int cols = 0,
            int rows = 0,
            int formatID = 6,
            char controlChar = '_') : base(positionX, positionY)
        {
            Base = typeof(Barcode2D);
            Content = content;
            Orientation = orientation;
            DMHeight = height;
            Quality = quality;
            Cols = cols;
            Rows = rows;
            FormatID = formatID;
            ControlChar = controlChar;
        }

        public BarcodeDatamatrix(string properties, byte[] elementBytes)
        {
            Base = typeof(Barcode2D);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            AddFieldData = false;

            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");
            try
            {
                //N,33,200,0,0,1,_,1
                var sp = this.properties.Split(',');
                if (sp.Length > 0)
                    Orientation = sp[0];
                if (sp.Length > 1)
                    DMHeight = Convert.ToInt32(sp[1]);
                if (sp.Length > 2)
                    Quality = Convert.ToInt32(sp[2]);
                if (sp.Length > 3)
                    Cols = Convert.ToInt32(sp[3]);
                if (sp.Length > 4)
                    Rows = Convert.ToInt32(sp[4]);
                if (sp.Length > 5)
                    FormatID = Convert.ToInt32(sp[5]);
                if (sp.Length > 6)
                    ControlChar = sp[6].ToCharArray()[0];
            }
            catch (Exception ex)
            {
            }
        }

        public static BarcodeDatamatrix Current
        {
            get => _current ?? (_current = new BarcodeDatamatrix(0, 0, ""));
            set => _current = value;
        }

        public string Orientation { get; set; } = "N";
        public int DMHeight { get; set; }
        public int Quality { get; set; } //0,50,80,100,150,200
        public int Cols { get; set; } //9-49
        public int Rows { get; set; } //9-94
        public int FormatID { get; set; }
        public char ControlChar { get; set; }
        public string Content { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^BX" + Orientation + "," + context.Scale(DMHeight) + "," + Quality + "," + Cols + "," + Rows +
                       "," + FormatID + "," + ControlChar);
            if (AddFieldData)
                result.Add("^FD" + Quality + "M," + Content + "^FS");

            return result;
        }
    }
}