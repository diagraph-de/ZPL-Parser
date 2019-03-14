using System; 
using System.Collections.Generic; 
using System.Drawing;
using System.Drawing.Imaging; 
using System.Linq; 
using System.Text; 

namespace ZPLParser
{ 
    public class ZPLImageConverter
    {
        public static ZPLImage ZPLfromBitmap(Bitmap bmp, bool createBody = true, bool compressHex = false)
        {
            var ZPLImage = new ZPLImage() { compressed = compressHex };

            ZPLImageConverter zp = new ZPLImageConverter();
            zp.CompressHex = true;
            zp.BlacknessLimitPercentage = 50;
            ZPLImage.Result = zp.ConvertfromImg(bmp, createBody, compressHex);
            ZPLImage.TotalBytes = zp.total;
            ZPLImage.WidthBytes = zp.widthBytes;

            return ZPLImage;
        }

        private bool _instanceFieldsInitialized = false;

        public struct ZPLImage
        {
            public int TotalBytes;
            public int WidthBytes;
            public string Result;
            public bool compressed;
            public byte[] bytes;
        }

        public ZPLImageConverter()
        {
            if (!_instanceFieldsInitialized)
            {
                InitializeInstanceFields();
                _instanceFieldsInitialized = true;
            }
        }


        private int blackLimit = 380;
        public int total;
        public int widthBytes; 
        private bool compressHex; 
        private bool compressBinary; 

        private static IDictionary<int, string> mapCode = new Dictionary<int, string>(); 

        public Image ZPLToBitmap(ZPLImage image, bool compressed)
        {
            Bitmap ret = new Bitmap(1, 1);
            try
            {
                byte[] grfData = image.bytes;
                int width = image.WidthBytes * 8;
                int height = image.TotalBytes / image.WidthBytes;



                ret = (Bitmap)GetImage(grfData, width, height, compressed);
            }
            catch (Exception ex)
            {
            }

            return ret;
        } 

        public  string ConvertfromImg(Bitmap image, bool withBody = true, bool hexCompress=false, bool binaryCompress=false)
        {
            compressHex = hexCompress;
            compressBinary = binaryCompress;
            string body = CreateBody(image);
            if (compressHex) 
                body = EncodeHexAscii(body);
            else if (binaryCompress)
            {
                //ToDo:
                //HEX BINARY
                //FF FF 1111 1111 1111 1111
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //80 01 1000 0000 0000 0001
                //FF FF 1111 1111 1111 1111

                //The above illustration is the binary view of the file that is attached to this knowledgebase.
                //        The parameters for the GF command in this example.
                //        ^ GF
                //        B Binary data being sent.
                //32 This is the total number of bytes to be transmitted for the total image.
                //32 This is the total number of bytes comprising the graphic format
                //2 This is the number of bytes in the download data that comprise one ROW
                //        of the image

            }
            if (withBody)
                return HeadDoc() + body + FootDoc();
            return body.Substring(1);
        }
         
        private String CreateBody(Bitmap bitmapImage)
        {
            StringBuilder sb = new StringBuilder();
            int height = bitmapImage.Height;
            int width = bitmapImage.Width;
            int red, green, blue, index = 0;
            char[] auxBinaryChar = { '0', '0', '0', '0', '0', '0', '0', '0' };

            widthBytes = width / 8;
            if (width % 8 > 0) 
                widthBytes = (((int)(width / 8)) + 1); 
            else 
                widthBytes = width / 8;
          
            total = widthBytes * height;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    var rgb = bitmapImage.GetPixel(w, h);
                    red = rgb.R;
                    green = rgb.G;
                    blue = rgb.B;
                    char auxChar = '1';
                    int totalColor = red + green + blue;
                    if (totalColor > blackLimit)
                    {
                        auxChar = '0';
                    }
                    auxBinaryChar[index] = auxChar;
                    index++;
                    if (index == 8 || w == (width - 1))
                    {
                        sb.Append(BinaryToHexString(new String(auxBinaryChar)));
                        auxBinaryChar = new char[] { '0', '0', '0', '0', '0', '0', '0', '0' };
                        index = 0;
                    }
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private string BinaryToHexString(string binaryStr)
        {
            int @decimal = Convert.ToInt32(binaryStr, 2);
            if (@decimal > 15)
            {
                return Convert.ToString(@decimal, 16).ToUpper();
            }
            else
            {
                return "0" + Convert.ToString(@decimal, 16).ToUpper();
            }
        }

        private string HexToBinaryString(string hexstring)
        {
            string binarystring = "";
            try
            {
                binarystring = String.Join(String.Empty,
                                                  hexstring.Select(
                                                                   c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                                                                  )
                                                 );
            }
            catch (Exception ex)
            { 
            }

            return binarystring;
        }
         
        private string HeadDoc()
        {
            string str = "^XA " +
                            "^FO0,0^GFA," + total + "," + total + "," + widthBytes + ", ";
            return str;
        }
        private string FootDoc()
        {
            string str = "^FS" +
                            "^XZ";
            return str;
        }
        public bool CompressHex
        {
            set
            {
                this.compressHex = value;
            }
        }
        public virtual int BlacknessLimitPercentage
        {
            set
            {
                blackLimit = (value * 768 / 100);
            }
        }

        private void InitializeInstanceFields()
        {
            mapCode[1] = "G";
            mapCode[2] = "H";
            mapCode[3] = "I";
            mapCode[4] = "J";
            mapCode[5] = "K";
            mapCode[6] = "L";
            mapCode[7] = "M";
            mapCode[8] = "N";
            mapCode[9] = "O";
            mapCode[10] = "P";
            mapCode[11] = "Q";
            mapCode[12] = "R";
            mapCode[13] = "S";
            mapCode[14] = "T";
            mapCode[15] = "U";
            mapCode[16] = "V";
            mapCode[17] = "W";
            mapCode[18] = "X";
            mapCode[19] = "Y";
            mapCode[20] = "g";
            mapCode[40] = "h";
            mapCode[60] = "i";
            mapCode[80] = "j";
            mapCode[100] = "k";
            mapCode[120] = "l";
            mapCode[140] = "m";
            mapCode[160] = "n";
            mapCode[180] = "o";
            mapCode[200] = "p";
            mapCode[220] = "q";
            mapCode[240] = "r";
            mapCode[260] = "s";
            mapCode[280] = "t";
            mapCode[300] = "u";
            mapCode[320] = "v";
            mapCode[340] = "w";
            mapCode[360] = "x";
            mapCode[380] = "y";
            mapCode[400] = "z";
        } 

        private string EncodeHexAscii(string code)
        {
            int line_max = widthBytes * 2;
            StringBuilder sbCode = new StringBuilder();
            StringBuilder line = new StringBuilder();
            string previousLine = null;
            int counter = 1;
            char aux = code[0];
            bool firstChar = false;

            for (int i = 1; i < code.Length; i++)
            {
                if (firstChar)
                {
                    aux = code[i];
                    firstChar = false;
                    continue;
                }
                if (code[i] == '\n')
                {
                    if (counter >= line_max && aux == '0') 
                        line.Append(","); 

                    else if (counter >= line_max && aux == 'F') 
                        line.Append("!"); 

                    else if (counter > 20)
                    {
                        int multi20 = (counter / 20) * 20;
                        int resto20 = (counter % 20);
                        line.Append(mapCode[multi20]);
                        if (resto20 != 0) 
                            line.Append(mapCode[resto20] + aux); 
                        else 
                            line.Append(aux); 
                    }
                    else
                    {
                        line.Append(mapCode[counter] + aux); 
                    }
                    counter = 1;
                    firstChar = true;
                    if (line.ToString().Equals(previousLine)) 
                        sbCode.Append(":"); 
                    else 
                        sbCode.Append(line.ToString()); 

                    previousLine = line.ToString();
                    line.Length = 0;
                    continue;
                }
                if (aux == code[i])
                {
                    counter++;
                }
                else
                {
                    if (counter > 20)
                    {
                        int multi20 = (counter / 20) * 20;
                        int resto20 = (counter % 20);
                        line.Append(mapCode[multi20]);

                        if (resto20 != 0) 
                            line.Append(mapCode[resto20] + aux); 
                        else 
                            line.Append(aux); 
                    }
                    else
                    {
                        line.Append(mapCode[counter] + aux);
                    }
                    counter = 1;
                    aux = code[i];
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

            var rawString = Encoding.ASCII.GetString(grfData);
            var sb = new StringBuilder();
            string previousLine = "";
            string line = ""; 

            var repeatsDictionary = new Dictionary<char, int>();
            foreach (KeyValuePair<int, string> kvp in mapCode) 
                repeatsDictionary.Add(kvp.Value.ToCharArray()[0], kvp.Key);
           
            var lineLength = width / 8*2;
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
                            for (int j = line.Length; j < lineLength; j++)
                                line += "0";
                            line += '\n';
                            previousLine = line;
                            sb.Append(previousLine);
                            line = "";
                            break;
                        case '!':
                            //a line with 1 
                            for (int j = line.Length; j < lineLength; j++)
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
                            for (int j = 0; j < charCnt; j++)
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

        public Image GetImage(byte[] grfData, int width, int height, bool compressed)
        { 
            Bitmap bitmapImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            if (compressed)
            {
                var decodedString = DecodeHexAscii(grfData, width, height);
                grfData = Encoding.ASCII.GetBytes(decodedString); 
            } 

            //remove '\n'
            grfData = RemoveLineBreak(grfData);

            int currentBit = 0;
            int currentByte = 0;
            var binString = "";  

            for (int h = 0; h < bitmapImage.Height; h++)
            { 
                for (int w = 0; w < bitmapImage.Width*2; w++)
                {
                    bitmapImage.SetPixel(w / 2, h, Color.White);

                    try
                    {
                        if (currentBit==0) 
                        { 
                            var hex = ((char)grfData[currentByte]).ToString(); 
                            hex += ((char)grfData[currentByte + 1]).ToString();
                            binString = HexToBinaryString(hex);
                        } 

                        if (binString.Substring(currentBit,1)=="1") 
                            bitmapImage.SetPixel(w/2, h, Color.Black);   
                          
                        currentBit++;
                        if (currentBit > 7)
                        {
                            currentByte += 1;
                            currentBit = 0;
                        }  
                    }
                    catch (Exception ex)
                    { 
                        break;
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


            ////Create zpl image to use with http://labelary.com/viewer.html
            //var bmp = new Bitmap(@"C:\Users\daniel\Desktop\bmp\zpl_monochrome.bmp");
            //var bytes = ConvertfromImg(bmp, true, false);
            //File.WriteAllBytes(@"C:\Users\daniel\Desktop\bmp\zpl_monochrome.prn", System.Text.Encoding.ASCII.GetBytes(bytes));


            return bitmapImage;
        }

        private byte[] RemoveLineBreak(byte[] grfData)
        {
            var ret = new List<byte>();
            foreach (byte var in grfData)
            {
                if(var==13 || var ==10)
                    continue;
                ret.Add(var);
            }
            return ret.ToArray();
        }
    }

}