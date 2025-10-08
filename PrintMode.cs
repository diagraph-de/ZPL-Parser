using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class PrintMode : BaseElement
    {
        private static PrintMode _current;
        private readonly string properties;
        private byte[] elementBytes;

        public PrintMode(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            var sp = this.properties.Split(',');

            if (sp[0].ToUpper() == Enums.PrintMode.T.ToString())
                DesiredMode = Enums.PrintMode.T;
            if (sp[0].ToUpper() == Enums.PrintMode.P.ToString())
                DesiredMode = Enums.PrintMode.P;
            if (sp[0].ToUpper() == Enums.PrintMode.R.ToString())
                DesiredMode = Enums.PrintMode.R;
            if (sp[0].ToUpper() == Enums.PrintMode.A.ToString())
                DesiredMode = Enums.PrintMode.A;
            if (sp[0].ToUpper() == Enums.PrintMode.C.ToString())
                DesiredMode = Enums.PrintMode.C;

            if (sp.Length > 1)
                PrepeelSelect = sp[1].ToUpper() == Enums.YesNo.Y.ToString()
                    ? Enums.YesNo.Y
                    : Enums.YesNo.N;
        }

        public PrintMode(Enums.PrintMode desiredMode = Enums.PrintMode.T, Enums.YesNo prepeelSelect = Enums.YesNo.Y)
        {
            DesiredMode = desiredMode;
            PrepeelSelect = prepeelSelect;
        }

        public static PrintMode Current
        {
            get => _current ?? (_current = new PrintMode());
            set => _current = value;
        }

        public Enums.PrintMode DesiredMode { get; protected set; }
        public Enums.YesNo PrepeelSelect { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^MMa,b 
            var result = new List<string>();
            result.Add("^MM" + DesiredMode + ',' + PrepeelSelect);
            return result;
        }
    }
}