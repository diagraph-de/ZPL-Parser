using System;
using System.Collections.Generic;

namespace Diagraph.Labelparser.ZPL
{
    public class PrintQuantity : BaseElement
    {
        private static PrintQuantity _current;
        private readonly string properties;
        private byte[] elementBytes;

        public PrintQuantity(string properties, byte[] elementBytes)
        {
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');
            TotalQuantity = Convert.ToInt32(sp[0]);
            Pause = Convert.ToInt32(sp[1]);
            Replicates = Convert.ToInt32(sp[2]);

            OverridePauseCount = sp[3].ToUpper() == Enums.YesNo.Y.ToString()
                ? Enums.YesNo.Y
                : Enums.YesNo.N;
        }

        public PrintQuantity(int totalQuantity, int pause, int replicates,
            Enums.YesNo overridePauseCount = Enums.YesNo.N)
        {
            TotalQuantity = totalQuantity;
            Pause = pause;
            Replicates = replicates;
            OverridePauseCount = overridePauseCount;
        }

        public static PrintQuantity Current
        {
            get => _current ?? (_current = new PrintQuantity(0, 0, 0));
            set => _current = value;
        }

        public int TotalQuantity { get; protected set; }
        public int Pause { get; protected set; }
        public int Replicates { get; protected set; }
        public Enums.YesNo OverridePauseCount { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^PQq,p,r,o 
            var result = new List<string>();
            result.Add("^PQ" + TotalQuantity + ',' + Pause + ',' + Replicates + ',' + OverridePauseCount);
            return result;
        }
    }
}