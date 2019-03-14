namespace ZebraParser.Parser.Printing
{
    public interface IZebraPrinter
    {
        PrinterSettings Settings { get; set; }
        void Print(byte[] Data);
    }

    public class PrinterSettings
    {
        public PrinterSettings()
        {
            RamDrive = 'R';
        }

        public int id { get; set; }
        public char PrinterType { get; set; }
        public string PrinterName { get; set; }
        public int PrinterPort { get; set; }
        public int AlignLeft { get; set; }
        public int AlignTop { get; set; }
        public int AlignTearOff { get; set; }
        public int Darkness { get; set; }
        public int PrintSpeed { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public char RamDrive { get; set; }
    }
}