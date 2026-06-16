using System;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Diagraph.Labelparser.ZPL;

public class ImageHelper
{
    private static readonly Dictionary<int, string> MapCode = new()
    {
        { 1, "G" },
        { 2, "H" },
        { 3, "I" },
        { 4, "J" },
        { 5, "K" },
        { 6, "L" },
        { 7, "M" },
        { 8, "N" },
        { 9, "O" },
        { 10, "P" },
        { 11, "Q" },
        { 12, "R" },
        { 13, "S" },
        { 14, "T" },
        { 15, "U" },
        { 16, "V" },
        { 17, "W" },
        { 18, "X" },
        { 19, "Y" },
        { 20, "g" },
        { 40, "h" },
        { 60, "i" },
        { 80, "j" },
        { 100, "k" },
        { 120, "l" },
        { 140, "m" },
        { 160, "n" },
        { 180, "o" },
        { 200, "p" },
        { 220, "q" },
        { 240, "r" },
        { 260, "s" },
        { 280, "t" },
        { 300, "u" },
        { 320, "v" },
        { 340, "w" },
        { 360, "x" },
        { 380, "y" },
        { 400, "z" }
    };

    private int blackLimit = 380;
    private bool compressHex;
    public int total;
    public int widthBytes;

    public bool CompressHex
    {
        set => compressHex = value;
    }

    public virtual int BlacknessLimitPercentage
    {
        set => blackLimit = value;
        // blackLimit = (value * 768 / 100);
    }

    public static strucZPL ZPLfromBitmap(Bitmap bmp, bool createBody = true, bool compressHex = false)
    {
        var ZPLImage = new strucZPL { compressed = compressHex };

        var zp = new ImageHelper
        {
            CompressHex = true,
            BlacknessLimitPercentage = 380
        };
        ZPLImage.Result = zp.ConvertfromImg(bmp, createBody, compressHex);
        ZPLImage.TotalBytes = zp.total;
        ZPLImage.WidthBytes = zp.widthBytes;

        return ZPLImage;
    }


    private string ConvertfromImg(Bitmap bmp, bool createBody, bool compressHex)
    {
        var hexAscii = BitmapToString(bmp);
        if (compressHex) hexAscii = EncodeHexAscii(hexAscii);

        var zplCode = "^GFA," + total + "," + total + "," + widthBytes + ", " + hexAscii;

        if (createBody)
        {
            var header = "^XA " + "^FO0,0^GFA," + total + "," + total + "," + widthBytes + ", ";
            var footer = "^FS" + "^XZ";
            zplCode = header + zplCode + footer;
        }

        return zplCode;
    }

    public string BitmapToString(Bitmap image, bool compress)
    {
        var hexAscii = BitmapToString(image);
        compressHex = compress;
        if (compressHex) hexAscii = EncodeHexAscii(hexAscii);
        return hexAscii;
    }


    public static string BitmapToString(Bitmap bmpSource)
    {
        if (bmpSource == null)
            return "";

        var dim = new Rectangle(Point.Empty, bmpSource.Size);
        var stride = (dim.Width + 7) / 8;
        var bytes = stride * dim.Height;

        using (var bmpCompressed = bmpSource.Clone(dim, PixelFormat.Format1bppIndexed))
        {
            var result = new StringBuilder();

            var imageData = GetImageData(dim, stride, bmpCompressed);

            byte[] previousRow = null;
            foreach (var row in imageData)
            {
                AppendLine(row, previousRow, result);
                previousRow = row;
            }

            return result.ToString();
        }
    }

    public static string GetGrfStoreCommand(Bitmap bmpSource, string fileName)
    {
        if (bmpSource == null) throw new ArgumentNullException("bmpSource");

        var dim = new Rectangle(Point.Empty, bmpSource.Size);
        var stride = (dim.Width + 7) / 8;
        var bytes = stride * dim.Height;

        using (var bmpCompressed = bmpSource.Clone(dim, PixelFormat.Format1bppIndexed))
        {
            var result = new StringBuilder();

            result.AppendFormat("^XA~DG{2},{0},{1},", stride * dim.Height, stride, fileName);
            var imageData = GetImageData(dim, stride, bmpCompressed);

            byte[] previousRow = null;
            foreach (var row in imageData)
            {
                AppendLine(row, previousRow, result);
                previousRow = row;
            }

            result.Append(@"^FS^XZ");

            return result.ToString();
        }
    }

    private static unsafe byte[][] GetImageData(Rectangle dim, int stride, Bitmap bmpCompressed)
    {
        byte[][] imageData;
        var data = bmpCompressed.LockBits(dim, ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
        try
        {
            var pixelData = (byte*)data.Scan0.ToPointer();
            var rightMask = (byte)(0xff << (data.Stride * 8 - dim.Width));
            imageData = new byte[dim.Height][];

            for (var row = 0; row < dim.Height; row++)
            {
                var rowStart = pixelData + row * data.Stride;
                imageData[row] = new byte[stride];

                for (var col = 0; col < stride; col++)
                {
                    var f = (byte)(0xff ^ rowStart[col]);
                    f = col == stride - 1 ? (byte)(f & rightMask) : f;
                    imageData[row][col] = f;
                }
            }
        }
        finally
        {
            bmpCompressed.UnlockBits(data);
        }

        return imageData;
    }

    private static void AppendLine(byte[] row, byte[] previousRow, StringBuilder baseStream)
    {
        if (row.All(r => r == 0))
        {
            baseStream.Append(",");
            return;
        }

        if (row.All(r => r == 0xff))
        {
            baseStream.Append("!");
            return;
        }

        if (previousRow != null && MatchByteArray(row, previousRow))
        {
            baseStream.Append(":");
            return;
        }

        var nibbles = new byte[row.Length * 2];
        for (var i = 0; i < row.Length; i++)
        {
            nibbles[i * 2] = (byte)(row[i] >> 4);
            nibbles[i * 2 + 1] = (byte)(row[i] & 0x0f);
        }

        for (var i = 0; i < nibbles.Length; i++)
        {
            var cPixel = nibbles[i];

            var repeatCount = 0;
            for (var j = i; j < nibbles.Length && repeatCount <= 400; j++)
                if (cPixel == nibbles[j])
                    repeatCount++;
                else
                    break;

            if (repeatCount > 2)
            {
                if (repeatCount == nibbles.Length - i
                    && (cPixel == 0 || cPixel == 0xf))
                {
                    if (cPixel == 0)
                    {
                        if (i % 2 == 1) baseStream.Append("0");
                        baseStream.Append(",");
                        return;
                    }

                    if (cPixel == 0xf)
                    {
                        if (i % 2 == 1) baseStream.Append("F");
                        baseStream.Append("!");
                        return;
                    }
                }
                else
                {
                    baseStream.Append(GetRepeatCode(repeatCount));
                    i += repeatCount - 1;
                }
            }

            baseStream.Append(cPixel.ToString("X"));
        }
    }

    private static string GetRepeatCode(int repeatCount)
    {
        if (repeatCount > 419)
            throw new ArgumentOutOfRangeException();

        var high = repeatCount / 20;
        var low = repeatCount % 20;

        const string lowString = " GHIJKLMNOPQRSTUVWXY";
        const string highString = " ghijklmnopqrstuvwxyz";

        var repeatStr = "";
        if (high > 0) repeatStr += highString[high];
        if (low > 0) repeatStr += lowString[low];

        return repeatStr;
    }

    private static bool MatchByteArray(byte[] row, byte[] previousRow)
    {
        for (var i = 0; i < row.Length; i++)
            if (row[i] != previousRow[i])
                return false;

        return true;
    }
    ///// <summary>
    ///// decode ASCII hexadecimal to Bitmap
    ///// </summary>
    ///// <param name="bitmapImage"></param>
    ///// <returns></returns>
    //private String BitmapToString(Bitmap bitmapImage)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    int height = bitmapImage.Height;
    //    int width = bitmapImage.Width;
    //    int rgb, red, green, blue, index = 0;
    //    var auxBinaryChar = new char[] { '0', '0', '0', '0', '0', '0', '0', '0' };
    //    widthBytes = width / 8;
    //    if (width % 8 > 0)
    //    {
    //        widthBytes = (width / 8) + 1;
    //    }
    //    else
    //    {
    //        widthBytes = width / 8;
    //    }
    //    total = widthBytes * height;
    //    for (int h = 0; h < height; h++)
    //    {
    //        for (int w = 0; w < width; w++)
    //        {
    //            rgb = bitmapImage.GetPixel(w, h).ToArgb();
    //            red = (rgb >> 16) & 0x000000FF;
    //            green = (rgb >> 8) & 0x000000FF;
    //            blue = (rgb) & 0x000000FF;
    //            char auxChar = '1';
    //            int totalColor = red + green + blue;
    //            if (totalColor > blackLimit)
    //            {
    //                auxChar = '0';
    //            }
    //            auxBinaryChar[index] = auxChar;
    //            index++;
    //            if (index == 8 || w == (width - 1))
    //            {
    //                sb.Append(FourByteBinary(new String(auxBinaryChar)));
    //                auxBinaryChar = new char[] { '0', '0', '0', '0', '0', '0', '0', '0' };
    //                index = 0;
    //            }
    //        }
    //        sb.Append("\n");
    //    }
    //    return sb.ToString();
    //}

    /// <summary>
    ///     Converts binary into integer representation of two hex digits
    /// </summary>
    /// <param name="binaryStr"></param>
    /// <returns></returns>
    private string FourByteBinary(string binaryStr)
    {
        var value = Convert.ToInt32(binaryStr, 2);
        if (value > 15)
            return Convert.ToString(value, 16).ToUpper();
        return "0" + Convert.ToString(value, 16).ToUpper();
    }

    public static byte[] DecompressZb64(string compressedString)
    {
        var b64 = Convert.FromBase64String(compressedString.Split(':')[0]).Skip(2).ToArray();
        return Decompress(b64);
    }

    public static byte[] Decompress(byte[] data)
    {
        byte[] decompressedArray = null;
        try
        {
            using (var decompressedStream = new MemoryStream())
            {
                using (var compressStream = new MemoryStream(data))
                {
                    using (var deflateStream = new DeflateStream(compressStream, CompressionMode.Decompress))
                    {
                        deflateStream.CopyTo(decompressedStream);
                    }
                }

                decompressedArray = decompressedStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            // do something !
        }

        return decompressedArray;
    }

    public Image ZPLToBitmap(strucZPL image, bool compressed)
    {
        var ret = new Bitmap(1, 1);
        try
        {
            var grfData = image.bytes;
            var width = image.WidthBytes * 8;
            var height = image.TotalBytes / image.WidthBytes;

            ret = (Bitmap)BinaryToBitmap(grfData, width, height, compressed);


            //ret.Save(@"C:\Users\d.frede\Desktop\backup.bmp");
        }
        catch (Exception ex)
        {
        }

        return ret;
    }

    //public string ConvertfromImg(Bitmap image, bool withBody = true, bool hexCompress = false, bool binaryCompress = false)
    //{
    //    compressHex = hexCompress;
    //    compressBinary = binaryCompress;
    //    string body = CreateBody(image);
    //    if (compressHex)
    //        body = EncodeHexAscii(body);
    //    else if (binaryCompress)
    //    {
    //        //ToDo:
    //        //HEX BINARY
    //        //FF FF 1111 1111 1111 1111
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //80 01 1000 0000 0000 0001
    //        //FF FF 1111 1111 1111 1111

    //        //The above illustration is the binary view of the file that is attached to this knowledgebase.
    //        //        The parameters for the GF command in this example.
    //        //        ^ GF
    //        //        B Binary data being sent.
    //        //32 This is the total number of bytes to be transmitted for the total image.
    //        //32 This is the total number of bytes comprising the graphic format
    //        //2 This is the number of bytes in the download data that comprise one ROW
    //        //        of the image

    //    }
    //    if (withBody)
    //        return HeadDoc() + body + FootDoc();
    //    return body.Substring(1);
    //}

    //private String CreateBody(Bitmap bitmapImage)
    //{
    //    StringBuilder sb = new StringBuilder();
    //    int height = bitmapImage.Height;
    //    int width = bitmapImage.Width;
    //    int red, green, blue, index = 0;
    //    char[] auxBinaryChar = { '0', '0', '0', '0', '0', '0', '0', '0' };

    //    widthBytes = width / 8;
    //    if (width % 8 > 0)
    //        widthBytes = (((int)(width / 8)) + 1);
    //    else
    //        widthBytes = width / 8;

    //    total = widthBytes * height;
    //    for (int h = 0; h < height; h++)
    //    {
    //        for (int w = 0; w < width; w++)
    //        {
    //            var rgb = bitmapImage.GetPixel(w, h);
    //            red = rgb.R;
    //            green = rgb.G;
    //            blue = rgb.B;
    //            char auxChar = '1';
    //            int totalColor = red + green + blue;
    //            if (totalColor > blackLimit)
    //            {
    //                auxChar = '0';
    //            }
    //            auxBinaryChar[index] = auxChar;
    //            index++;
    //            if (index == 8 || w == (width - 1))
    //            {
    //                sb.Append(BinaryToHexString(new String(auxBinaryChar)));
    //                auxBinaryChar = new char[] { '0', '0', '0', '0', '0', '0', '0', '0' };
    //                index = 0;
    //            }
    //        }
    //        sb.Append("\n");
    //    }
    //    return sb.ToString();
    //}

    //private string HeadDoc()
    //{
    //    string str = "^XA " +
    //                    "^FO0,0^GFA," + total + "," + total + "," + widthBytes + ", ";
    //    return str;
    //}
    //private string FootDoc()
    //{
    //    string str = "^FS" +
    //                    "^XZ";
    //    return str;
    //}

    //private string BinaryToHexString(string binaryStr)
    //{
    //    int @decimal = Convert.ToInt32(binaryStr, 2);
    //    if (@decimal > 15)
    //    {
    //        return Convert.ToString(@decimal, 16).ToUpper();
    //    }
    //    else
    //    {
    //        return "0" + Convert.ToString(@decimal, 16).ToUpper();
    //    }
    //}

    private string HexToBinaryString(string hexstring)
    {
        var binarystring = "";
        try
        {
            binarystring = string.Join(string.Empty,
                hexstring.Select(c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );
        }
        catch (Exception ex)
        {
        }

        return binarystring;
    }

    /// <summary>
    /// </summary>
    /// <param name="compress to ZB64 encoded string"></param>
    /// <returns></returns>
    private string EncodeHexAscii(string code)
    {
        var maxlinea = widthBytes * 2;
        var sbCode = new StringBuilder();
        var sbLinea = new StringBuilder();
        string previousLine = null;
        var counter = 1;
        var aux = code.ElementAt(0);
        var firstChar = false;
        for (var i = 1; i < code.Length; i++)
        {
            if (firstChar)
            {
                aux = code.ElementAt(i);
                firstChar = false;
                continue;
            }

            if (code.ElementAt(i) == '\n')
            {
                if (counter >= maxlinea && aux == '0')
                {
                    sbLinea.Append(",");
                }
                else if (counter >= maxlinea && aux == 'F')
                {
                    sbLinea.Append("!");
                }
                else if (counter > 20)
                {
                    var multi20 = counter / 20 * 20;
                    var resto20 = counter % 20;
                    sbLinea.Append(MapCode[multi20]);
                    if (resto20 != 0)
                        sbLinea.Append(MapCode[resto20]).Append(aux);
                    else
                        sbLinea.Append(aux);
                }
                else
                {
                    sbLinea.Append(MapCode[counter]).Append(aux);
                }

                counter = 1;
                firstChar = true;
                if (sbLinea.ToString().Equals(previousLine))
                    sbCode.Append(":");
                else
                    sbCode.Append(sbLinea);
                previousLine = sbLinea.ToString();
                sbLinea.Length = 0;
                continue;
            }

            if (aux == code.ElementAt(i))
            {
                counter++;
            }
            else
            {
                if (counter > 20)
                {
                    var multi20 = counter / 20 * 20;
                    var resto20 = counter % 20;
                    sbLinea.Append(MapCode[multi20]);
                    if (resto20 != 0)
                        sbLinea.Append(MapCode[resto20]).Append(aux);
                    else
                        sbLinea.Append(aux);
                }
                else
                {
                    sbLinea.Append(MapCode[counter]).Append(aux);
                }

                counter = 1;
                aux = code.ElementAt(i);
            }
        }

        return sbCode.ToString();
    }

    private string DecodeHexAscii(byte[] grfData, int width, int height)
    {
        //The following represent the repeat counts 1,2,3,4,5,...,19 on a subsequent Hexadecimal value.
        //        NOTE: Values start with G since 0 thru 9 and A thru F are already used for HEX values.)

        //G H I J K L M N O P  Q  R  S  T  U  V  W  X  Y
        //1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19

        //Example: Sending a M6 to the printer is identical to sending the following hexadecimal data:
        //6666666
        //The "M" has the value of 7.Therefore "M6" sends seven (7) hexadecimal 6's.

        // g  h   I  j  k   I   m   n   o   p   q   r   s   t   u   v   w   x   y   z
        //20  40  60 80 100 120 140 160 180 200 220 240 260 280 300 320 340 360 380 400

        //Example 1 • Sending M6 to the printer is identical to sending the following hexadecimal data: 
        //6 6 6 6 6 6 6 The M has the value of 7.Therefore M6 sends seven(7) hexadecimal 6's. 
        //
        //Example 2 • Sending hB to the printer is identical to sending the following hexadecimal data: 
        //BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB The h has a value of 40.Therefore, hB sends 40 Hexadecimal B's. 
        //
        // Example 3 • Sending MvB or vMB sends 327 hexadecimal B's to the printer. The M has a value of 7, and the v has a value of 320.Together,
        // they specify. 327 Hexadecimal B's. 
        //
        // Repeat Values Several repeat values can be used together to achieve any desired value.
        // • a comma (,) fills the line to the right with zeros(0) until the specified line byte is filled.
        // • an exclamation mark(!) fills the line, to the right, with ones(1) until the specified line byte is filled.
        // • a colon(:) denotes repetition of the previous line. 

        var sb = new StringBuilder();
        var previousLine = "";
        var line = "";

        var repeatsDictionary = new Dictionary<char, int>();
        foreach (var kvp in MapCode)
            repeatsDictionary.Add(kvp.Value.ToCharArray()[0], kvp.Key);

        var lineLength = Math.Max(1, width / 4);
        var charCnt = 1;
        for (var i = 0; i < grfData.Length; i++)
        {
            var c = (char)grfData[i];
            if ("GHIJKLMNOPQRSTUVWXYghIjkImnopqrstuvwxyz".Contains(c))
            {
                charCnt += repeatsDictionary[c];
            }
            else
            {
                switch (c)
                {
                    case '\n':
                        line = "";
                        break;

                    case ',':
                        //a line with 0                        
                        for (var j = line.Length; j < lineLength; j++)
                            line += "0";
                        line += '\n';
                        previousLine = line;
                        sb.Append(previousLine);
                        line = "";
                        break;
                    case '!':
                        //a line with 1 
                        for (var j = line.Length; j < lineLength; j++)
                            line += "F";
                        line += '\n';
                        previousLine = line;
                        sb.Append(previousLine);
                        line = "";
                        break;

                    case ':':
                        //repeat previous line
                        sb.Append(previousLine);
                        line = "";
                        break;

                    default:
                        for (var j = 0; j < charCnt; j++)
                        {
                            line += c;
                            if (line.Length == lineLength)
                            {
                                line += '\n';
                                previousLine = line;
                                sb.Append(previousLine);
                                line = "";
                            }
                        }

                        break;
                }

                charCnt = 1;
            }
        }

        var decodeString = sb.ToString();
        return decodeString;
    }

    public Image BinaryToBitmap(byte[] grfData, int width, int height, bool compressed)
    {
        var bitmapWidth = Math.Max(1, width);
        var bitmapHeight = Math.Max(1, height);
        var bitmapImage = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);

        if (grfData == null || grfData.Length == 0)
            return bitmapImage;

        var asciiData = Encoding.ASCII.GetString(RemoveLineBreak(grfData));
        if (compressed)
            asciiData = DecodeHexAscii(Encoding.ASCII.GetBytes(asciiData), width, height);

        var hexData = new string(asciiData.Where(c => !char.IsWhiteSpace(c)).ToArray());
        if (hexData.Length < 2)
            return bitmapImage;

        var rawBytes = new List<byte>(hexData.Length / 2);
        for (var i = 0; i + 1 < hexData.Length; i += 2)
        {
            var hexPair = hexData.Substring(i, 2);
            if (byte.TryParse(hexPair, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedByte))
                rawBytes.Add(parsedByte);
        }

        var bytesPerRow = Math.Max(1, bitmapWidth / 8);
        for (var row = 0; row < bitmapHeight; row++)
        {
            for (var colByte = 0; colByte < bytesPerRow; colByte++)
            {
                var byteIndex = row * bytesPerRow + colByte;
                var value = byteIndex < rawBytes.Count ? rawBytes[byteIndex] : (byte)0x00;

                for (var bit = 0; bit < 8; bit++)
                {
                    var pixelX = colByte * 8 + bit;
                    if (pixelX >= bitmapWidth)
                        break;

                    var isBlack = (value & (0x80 >> bit)) != 0;
                    bitmapImage.SetPixel(pixelX, row, isBlack ? Color.Black : Color.White);
                }
            }
        }

        ////Save Debug image
        //var file = @"C:\Users\daniel\Desktop\bmp\Config.bmp";
        //if (File.Exists(file))
        //{
        //    File.Delete(file);
        //    Thread.Sleep(500);
        //} 
        //bitmapImage.Save(file);


        ////Create zpl image for preview testing
        //var bmp = new Bitmap(@"C:\Users\daniel\Desktop\bmp\zpl_monochrome.bmp");
        //var bytes = ConvertfromImg(bmp, true, false);
        //File.WriteAllBytes(@"C:\Users\daniel\Desktop\bmp\zpl_monochrome.prn", System.Text.Encoding.ASCII.GetBytes(bytes));


        return bitmapImage;
    }

    private byte[] RemoveLineBreak(byte[] grfData)
    {
        var ret = new List<byte>();
        foreach (var var in grfData)
        {
            if (var == 13 || var == 10)
                continue;
            ret.Add(var);
        }

        return ret.ToArray();
    }

    public struct strucZPL
    {
        public int TotalBytes;
        public int WidthBytes;
        public string Result;
        public bool compressed;
        public byte[] bytes;
    }
}
