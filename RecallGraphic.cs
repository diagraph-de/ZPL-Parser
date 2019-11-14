#region

using System;
using System.Collections.Generic;
using System.Drawing;

#endregion

namespace ZPLParser
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
            var props = this.properties.Split(',');

            StorageDevice = props[0].Split(':')[0] + ":";
            ImageName = props[0].Split(':')[1].Split('.')[0];
            Extension = "." + props[0].Split(':')[1].Split('.')[1];
            MagnificationFactorX = Convert.ToInt16(props[1]);
            MagnificationFactorY = Convert.ToInt16(props[2]);
        }

        public static RecallGraphic Current
        {
            get { return _current ?? (_current = new RecallGraphic(0, 0, 'R', "SAMPLE", ".GRF", 1, 1)); }
            set { _current = value; }
        }
        public RecallGraphic(int positionX, int positionY, char storageDevice, string imageName, string extension, int magnificationFactorX = 1,
            int magnificationFactorY = 1)
            : base(positionX, positionY)
        {
            StorageDevice = storageDevice + ":";
            ImageName = imageName;
            Extension = extension;
            MagnificationFactorX = magnificationFactorX;
            MagnificationFactorY = magnificationFactorY;
        }

        public Bitmap GetBitmap()
        {
            Bitmap bmp = new Bitmap(1, 1);

            return bmp;
        }

        public string StorageDevice { get; set; }
        public string ImageName { get; set; }
        public string Extension { get; set; }
        public int MagnificationFactorX { get; set; }
        public int MagnificationFactorY { get; set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            var result = new List<string>();
            if (Origin != null)
                result.AddRange(Origin.Render(context));
            result.Add(string.Format("^XG{0}{1}{2},{3},{4}", StorageDevice, ImageName, Extension, MagnificationFactorX, MagnificationFactorY));

            return result;
        }
    }
}