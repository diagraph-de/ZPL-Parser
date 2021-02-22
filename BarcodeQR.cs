using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class BarcodeQR : Barcode2D
    {
        private static BarcodeQR _current;
        private readonly string properties;
        private byte[] elementBytes;

        public BarcodeQR(
            int positionX,
            int positionY,
            string content,
            int model = 2,
            int magnificationFactor = 2,
            Enums.ErrorCorrection errorCorrection = Enums.ErrorCorrection.Q,
            int maskValue = 7) : base(positionX, positionY)
        {
            Base = typeof(Barcode2D);
            Content = content;
            Model = model;
            MagnificationFactor = magnificationFactor;
            ErrorCorrection = errorCorrection;
            MaskValue = maskValue;
        }

        public BarcodeQR(string properties, byte[] elementBytes)
        {
            Base = typeof(Barcode2D);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            AddFieldData = false;

            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');
            if (sp.Length > 0)
                FieldPosition = sp[0];
            if (sp.Length > 1)
                Model = Convert.ToInt32(sp[1]);
            if (sp.Length > 2)
                MagnificationFactor = Convert.ToInt32(sp[2]);
        }

        public static BarcodeQR Current
        {
            get { return _current ?? (_current = new BarcodeQR(0, 0, "")); }
            set { _current = value; }
        }

        public string FieldPosition { get; set; } = "N";
        public int Model { get; set; } = 2;
        public int MagnificationFactor { get; set; } //1-10
        public Enums.ErrorCorrection ErrorCorrection { get; set; }
        public int MaskValue { get; set; }
        public string Content { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FO100,100
            //^BQN,2,10
            //^FDMM,AAC - 42 ^ FS
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add("^BQ" + FieldPosition + "," + Model + "," + context.Scale(MagnificationFactor) + "," + ErrorCorrection + "," + MaskValue);
            if (AddFieldData)
                result.Add("^FD" + ErrorCorrection + "M," + Content + "^FS");

            return result;
        }
    }
}