#region

using System;
using System.Collections.Generic;
using System.Drawing;

#endregion

namespace Allen.Labelparser.ZPL
{
    /// <summary>
    ///     ^XGd:o.x,mx,my
    /// </summary>
    public class RecallGraphic : PositionedElement
    {
        private static RecallGraphic _current;
        private readonly string properties;
        private byte[] elementBytes;

        public RecallGraphic(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');

            StorageDevice = sp[0].Split(':')[0] + ":";
            ImageName = sp[0].Split(':')[1].Split('.')[0];
            Extension = "." + sp[0].Split(':')[1].Split('.')[1];
            MagnificationFactorX = Convert.ToInt16(sp[1]);
            MagnificationFactorY = Convert.ToInt16(sp[2]);
        }

        public RecallGraphic(int positionX, int positionY, char storageDevice, string imageName, string extension,
            int magnificationFactorX = 1,
            int magnificationFactorY = 1)
            : base(positionX, positionY)
        {
            StorageDevice = storageDevice + ":";
            ImageName = imageName;
            Extension = extension;
            MagnificationFactorX = magnificationFactorX;
            MagnificationFactorY = magnificationFactorY;
        }

        public static RecallGraphic Current
        {
            get => _current ?? (_current = new RecallGraphic(0, 0, 'R', "SAMPLE", ".GRF"));
            set => _current = value;
        }

        public string StorageDevice { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }
        public int MagnificationFactorX { get; set; }
        public int MagnificationFactorY { get; set; }

        public Bitmap GetBitmap()
        {
            var bmp = new Bitmap(1, 1);

            return bmp;
        }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add(string.Format("^XG{0}{1}{2},{3},{4}", StorageDevice, ImageName, Extension, MagnificationFactorX,
                MagnificationFactorY));

            return result;
        }
    }
}