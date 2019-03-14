using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class Comment : BaseElement
    {
        private static Comment _current;
        private readonly string properties;
        private byte[] elementBytes;

        public Comment(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            NonPrintingComment = this.properties.Split(',')[0];
        }

        public Comment(string comment = "")
        {
            NonPrintingComment = comment;
        }

        public static Comment Current
        {
            get { return _current ?? (_current = new Comment()); }
            set { _current = value; }
        }

        public string NonPrintingComment { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FXc 
            var result = new List<string>();
            result.Add(Environment.NewLine + "^FX" + NonPrintingComment);
            return result;
        }
    }
}