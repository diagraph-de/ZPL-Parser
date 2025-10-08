using System;

namespace Diagraph.Labelparser.ZPL
{
    public class LabelException : Exception
    {
        public LabelException()
        {
        }

        public LabelException(string message)
            : base(message)
        {
        }

        public LabelException(string message, Exception ex) : base(message, ex)
        {
        }

        public LabelException(string message, ZplParser.ElementCodeTypes elementcode)
            : base(message + " - Elementcode: " + elementcode)
        {
        }
    }
}