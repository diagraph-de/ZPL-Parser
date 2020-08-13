using System;
using System.Collections.Generic;

namespace ZPLParser
{
    public class FieldClock : FieldElement
    {
        private static FieldClock _currentPosition;
        private readonly string properties;
        private byte[] elementBytes;

        public FieldClock(string properties, byte[] elementBytes)
        {
            Base = typeof(FieldElement);
            this.properties = properties.Replace(Environment.NewLine, "");
            this.elementBytes = elementBytes;

            Current = this;
            
            while (this.properties.Contains(",,"))
                this.properties = this.properties.Replace(",,", ",0,");

            var sp = this.properties.Split(',');

            PrimaryClock = sp[0].ToCharArray()[0];
            if (sp.Length > 1)
                SecondaryClock = sp[1].ToCharArray()[1];
            if (sp.Length > 2)
                TertiaryClock = sp[2].ToCharArray()[2];
        }

        public FieldClock(char primaryClock, char secondaryClock, char tertiaryClock)
        {
            Base = typeof(FieldElement);
            PrimaryClock = primaryClock;
            SecondaryClock = secondaryClock;
            TertiaryClock = tertiaryClock;
        }

        public static FieldClock Current
        {
            get { return _currentPosition ?? (_currentPosition = new FieldClock('%', '{', '#')); }
            set { _currentPosition = value; }
        }

        public int PrimaryClock { get; protected set; }
        public int SecondaryClock { get; protected set; }
        public int TertiaryClock { get; protected set; }

        public override IEnumerable<string> Render(ZPLRenderOptions context)
        {
            //^FN0
            var result = new List<string>();
            result.Add("^FC" + PrimaryClock + "," + SecondaryClock + "," + TertiaryClock);
            return result;
        }
    }
}