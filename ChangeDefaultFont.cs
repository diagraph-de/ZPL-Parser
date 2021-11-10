using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class ChangeDefaultFont : BaseElement
    {
        private static ChangeDefaultFont _current;
        private readonly string properties;
        private byte[] elementBytes;

        public ChangeDefaultFont(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;

            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');

            FontNr = sp[0];
            CharacterHeight = Convert.ToInt32(sp[1]);
            if (sp.Length > 2)
                CharacterWidth = Convert.ToInt32(sp[2]);
        }

        public ChangeDefaultFont(string fontNr, int characterHeight, int characterWidth)
        {
            FontNr = fontNr;
            CharacterHeight = characterHeight;
            CharacterWidth = characterWidth;
        }

        public static ChangeDefaultFont Current
        {
            get => _current ?? (_current = new ChangeDefaultFont("Z", 9, 5));
            set => _current = value;
        }

        public string FontNr { get; protected set; }
        public int CharacterWidth { get; protected set; }
        public int CharacterHeight { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^CFf,h,w 
            var result = new List<string>();
            result.Add("^CF" + FontNr + ',' + CharacterHeight + ',' + CharacterWidth);
            return result;
        }
    }
}