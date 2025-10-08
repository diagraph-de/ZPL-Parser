#region

using System.Net.Sockets;

#endregion

namespace ZebraParser.Parser.Printing
{
    public class NetworkPrinter : IZebraPrinter
    {
        public NetworkPrinter(PrinterSettings settings)
        {
            Settings = settings;
        }

        public PrinterSettings Settings { get; set; }

        public void Print(byte[] data)
        {
            using (TcpClient printer = new TcpClient(Settings.PrinterName, Settings.PrinterPort))
            {
                using (NetworkStream strm = printer.GetStream())
                {
                    strm.Write(data, 0, data.Length);
                    strm.Close();
                }
                printer.Close();
            }
        }
    }
}