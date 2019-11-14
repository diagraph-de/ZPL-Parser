using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ZPLParser
{
    public class ImageHelper
    {
        public static strucZPL ZPLfromBitmap(Bitmap bmp, bool createBody = true, bool compressHex = false)
        {
            var ZPLImage = new strucZPL() { compressed = compressHex };

            ImageHelper zp = new ImageHelper
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
            String hexAscii = BitmapToString(bmp);
            if (compressHex)
            {
                hexAscii = EncodeHexAscii(hexAscii);
            }

            String zplCode = "^GFA," + total + "," + total + "," + widthBytes + ", " + hexAscii;

            if (createBody)
            {
                String header = "^XA " + "^FO0,0^GFA," + total + "," + total + "," + widthBytes + ", ";
                String footer = "^FS" + "^XZ";
                zplCode = header + zplCode + footer;
            }
            return zplCode;
        }

        public string BitmapToString(Bitmap image, bool compress)
        {
            String hexAscii = BitmapToString(image);
            compressHex = compress;
            if (compressHex)
            {
                hexAscii = EncodeHexAscii(hexAscii);
            }
            return hexAscii;
        }

        /// <summary>
        /// decode ASCII hexadecimal to Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        private String BitmapToString(Bitmap bitmapImage)
        {
            StringBuilder sb = new StringBuilder();
            int height = bitmapImage.Height;
            int width = bitmapImage.Width;
            int rgb, red, green, blue, index = 0;
            var auxBinaryChar = new char[] { '0', '0', '0', '0', '0', '0', '0', '0' };
            widthBytes = width / 8;
            if (width % 8 > 0)
            {
                widthBytes = (width / 8) + 1;
            }
            else
            {
                widthBytes = width / 8;
            }
            total = widthBytes * height;
            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    rgb = bitmapImage.GetPixel(w, h).ToArgb();
                    red = (rgb >> 16) & 0x000000FF;
                    green = (rgb >> 8) & 0x000000FF;
                    blue = (rgb) & 0x000000FF;
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
                        sb.Append(FourByteBinary(new String(auxBinaryChar)));
                        auxBinaryChar = new char[] { '0', '0', '0', '0', '0', '0', '0', '0' };
                        index = 0;
                    }
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts binary into integer representation of two hex digits 
        /// </summary>
        /// <param name="binaryStr"></param>
        /// <returns></returns>
        private String FourByteBinary(String binaryStr)
        {
            int value = Convert.ToInt32(binaryStr, 2);
            if (value > 15)
            {
                return Convert.ToString(value, 16).ToUpper();
            }
            else
            {
                return "0" + Convert.ToString(value, 16).ToUpper();
            }
        }

        public struct strucZPL
        {
            public int TotalBytes;
            public int WidthBytes;
            public string Result;
            public bool compressed;
            public byte[] bytes;
        }

        private int blackLimit = 380;
        public int total;
        public int widthBytes;
        private bool compressHex;
        private bool compressBinary;

        public Image ZPLToBitmap(strucZPL image, bool compressed)
        {

            Bitmap ret = new Bitmap(1, 1);
            try
            {
                byte[] grfData = image.bytes;
                int width = image.WidthBytes * 8;
                int height = image.TotalBytes / image.WidthBytes;

                ret = (Bitmap)BinaryToBitmap(grfData, width, height, compressed);
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
                blackLimit = value;
                // blackLimit = (value * 768 / 100);
            }
        }
        private static readonly Dictionary<int, String> MapCode = new Dictionary<int, String>()
        {
            {1, "G"},
            {2, "H"},
            {3, "I"},
            {4, "J"},
            {5, "K"},
            {6, "L"},
            {7, "M"},
            {8, "N"},
            {9, "O" },
            {10, "P"},
            {11, "Q"},
            {12, "R"},
            {13, "S"},
            {14, "T"},
            {15, "U"},
            {16, "V"},
            {17, "W"},
            {18, "X"},
            {19, "Y"},
            {20, "g"},
            {40, "h"},
            {60, "i"},
            {80, "j" },
            {100, "k"},
            {120, "l"},
            {140, "m"},
            {160, "n"},
            {180, "o"},
            {200, "p"},
            {220, "q"},
            {240, "r"},
            {260, "s"},
            {280, "t"},
            {300, "u"},
            {320, "v"},
            {340, "w"},
            {360, "x"},
            {380, "y"},
            {400, "z" }
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compress to ZB64 encoded string"></param>
        /// <returns></returns>
        private String EncodeHexAscii(String code)
        {
            int maxlinea = widthBytes * 2;
            StringBuilder sbCode = new StringBuilder();
            StringBuilder sbLinea = new StringBuilder();
            String previousLine = null;
            int counter = 1;
            char aux = code.ElementAt(0);
            bool firstChar = false;
            for (int i = 1; i < code.Length; i++)
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
                        int multi20 = (counter / 20) * 20;
                        int resto20 = (counter % 20);
                        sbLinea.Append(MapCode[multi20]);
                        if (resto20 != 0)
                        {
                            sbLinea.Append(MapCode[resto20]).Append(aux);
                        }
                        else
                        {
                            sbLinea.Append(aux);
                        }
                    }
                    else
                    {
                        sbLinea.Append(MapCode[counter]).Append(aux);
                    }
                    counter = 1;
                    firstChar = true;
                    if (sbLinea.ToString().Equals(previousLine))
                    {
                        sbCode.Append(":");
                    }
                    else
                    {
                        sbCode.Append(sbLinea.ToString());
                    }
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
                        int multi20 = (counter / 20) * 20;
                        int resto20 = (counter % 20);
                        sbLinea.Append(MapCode[multi20]);
                        if (resto20 != 0)
                        {
                            sbLinea.Append(MapCode[resto20]).Append(aux);
                        }
                        else
                        {
                            sbLinea.Append(aux);
                        }
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
            string previousLine = "";
            string line = "";

            var repeatsDictionary = new Dictionary<char, int>();
            foreach (KeyValuePair<int, string> kvp in MapCode)
                repeatsDictionary.Add(kvp.Value.ToCharArray()[0], kvp.Key);

            var lineLength = width;// / 8*2;
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

        public Image BinaryToBitmap(byte[] grfData, int width, int height, bool compressed)
        {
            //var grfString = "zzhQ0AhW07FCI01HFT015hW0HFCI01HFU06hW0HF8I03HFT013hW0HF8I03FEU0AhV01HF8I03FEiR01HFJ07FEiR01HFJ07FCiR03HFJ07FCiP0603FE7F80HFCO0FC01EhV0D07JFC0HF9HF07FCHF3HF0HF8hT01907JFE0HF9FE07FDKFBHFChT03107JFE1HFBFE07F9NFEhT0610LF1HF3FE0HF9NFEhT0C10LF1HF3FC0HFBOFhS01810IF7HF3HF7FC1HF3OFV0F80781C0F03EH0FH0F07C1F0780FEN030H1HFC3HF3FE7FC1HF3HF1HFE7FEU01FE0FC1C1E03E03FC3FC7C1F1FE0FEN01EH1HF81HF3FEHF81HF7FE1HF87FEU03CE1CE1C3C03E039C39C7E3F1CE0CP02C1HF01HF7FCHF83FE7FC1HF87FEU038738E1C3807F070E70E7E3F3871CP037BFE01HF7FCHF03FE7FC1HF07FCW07H0E1C7H0H7070E70E7E3F3871CP0713FE03FEHFDHF03FEHFC3HF07FCW0701C1CEH0H7070E70E76373871FCO0703FE03FEHF9HF07FCHF83HF0HFCW0E0381DEH0E3870E70EJ73873FEO0607FE07FCHF9FE07FCHF83FE0HF8V01E03C1FEH0E3870E70EJ738738EO0E07FE0HFDHFBHF0HFDHF87FE0HF8V03CH0E1HFH0E3870E70EJ7387H07O0E07HF3HF9HF3HF3HF9HF07FE1HF8V038H071F701HFC70E70E7367387H07O0C0LF9HF3KF9HF07FC1HFW07I071E381HFC70E70E73E7387H07N01C0LF3HF3KFBFE0HFC1HFW0FI071C3C1HFC70E70E73E7387H07N01C0KFE3FE3KF3FE0HF83HFV01E03871C1C380E70E70E73E7387387N0181KFC3FE3KF3FE1HF83FEV01C03CE1C0E380E39C39C73E71CE3CEN0381KF87FE1JFE7FC1HF83FEV03HF1FC1C0F380E3FC3FC71C71FE1FCN0383FE7FE07FC1HF9FE7FC1HF07FEV03HF0781C0H7H070FH0F071C70780F8T0FL07CzmY03iTFCL03iTFCL03iTFCzzlW01C71C71C71C71C71CiK01C71C71C71C71C71CiK01C71C71C71C71C71CiK01C0E381KFE3FE38iJ01C0E381KFE3FE38iJ01C0E381KFE3FE38iJ01C7E3F1C7I01F8FCiK01C7E3F1C7I01F8FCiK01C7E3F1C7I01F8FCiK01C0E070381C7I0E38iJ01C0E070381C7I0E38iJ01C0E070381C7I0E38iJ01F81C703F1C0EiO01F81C703F1C0EiO01F81C703F1C0EiO01F8LFEI03F1F8iJ01F8LFEI03F1F8iJ01F8LFEI03F1F8iJ01C01C7H07E38E3FiM01C01C7H07E38E3FiM01C01C7H07E38E3FiM01CI07I0E38E38HF8iJ01CI07I0E38E38HF8iJ01CI07I0E38E38HF8iJ01C7038E071C7E07EiL01C7038E071C7E07EgX03807H0E0HFE0E01C03807M01C7038E071C7E07EgX0FE1FC3F8HFE3F87F0FE1FCL01C7E071IFC01C0HF8U03C3FC1C0F03CO0HE1DC3B8HFE3B8H70HE1DCL01C7E071IFC01C0HF8U07E3FC7E1F87EN01C738E71CEH071CE39C738EL01C7E071IFC01C0HF8U0H601C67198E7N01C738E71CEH071CE39C738EL01F8E3F1HFE3F1IFCV0E7038E039CE7N01C738E71CHFC71CE39C738EL01F8E3F1HFE3F1IFCV0E7030FC39C07N01C738E71CHFC71CE39C738EL01F8E3F1HFE3F1IFCV0E7070FE39C1EN01C738E71CHFC71CE39C738EL01C7EH01HF03F1C7HF8U0E7060E739C1EN01C738E71CEH071CE39C738EL01C7EH01HF03F1C7HF8U0E70E0E739C07N01C738E71CEH071CE39C738EL01C7EH01HF03F1C7HF8U0E70E0E739C07N01C738E71CEH071CE39C738EL01C0HF8E38E3F1C71CV0E70C0E739CE7O0HE1DC3B8HFE3B8H70HE1DCL01C0HF8E38E3F1C71CV0H61C067198E7O0FE1FC3F8HFE3F87F0FE1FCL01C0HF8E38E3F1C71CV07E1C07E1F87EO03807H0E0HFE0E01C03807M01F8E3FI0IFE3IF8U03C1C03C0F03CgW01F8E3FI0IFE3IF8iJ01F8E3FI0IFE3IF8iJ01HF1C71F8H07038iM01HF1C71F8H07038iM01HF1C71F8H07038iM01HFI0E38FC01C7038iJ01HFI0E38FC01C7038iJ01HFI0E38FC01C7038iJ01F8HF8FC71HF038iM01F8HF8FC71HF038iM01F8HF8FC71HF038iM01F81F81C70381HF038iJ01F81F81C70381HF038iJ01F81F81C70381HF038iJ01C01F8I01C7HF81CiK01C01F8I01C7HF81CiK01C01F8I01C7HF81CiK01C0E38E07FCH03IF8iJ01C0E38E07FCH03IF8iJ01C0E38E07FCH03IF8iJ01C0EH0EH01C7JFCiK01C0EH0EH01C7JFCiK01C0EH0EH01C7JFCiK01VF8iJ01VF8iJ01VF8zpT0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98S0H67F9E6187H981E19E60H61819860H61981860H61860H61819860H607I9E18H67F98zlJ03FE03E0E0E1E0781E0781E0380E038hR03HF03E0E0E3F0FC3F0FC3F0FE3F8FEhR03HF83E0E0EH30HCH31CEH30CEH38CEhR03838H70E0E739CE739CE739C0701ChS03838H70E0E739CE7380E739F87E1F8hR03HF0H70E0E739CE7381C739FC7F1FChR03FE0E38E0E739CE7381C739CE739CEhR03HF0HF8E0E739CE73838739CE739CEhR03838HF8E0E739CE73870739CE739CEhR03839HFCF1E739CE73870739CE739CEhR03HF9C1C7FCH30HCH30E0H30CEH38CEhR03HF1C1C7FC3F0FC3F1FE3F0FC3F0FChR03FE380E1F01E0781E1FE1E0781E078zgV0";
            Bitmap bitmapImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            if (compressed)
            {
                var decodedString = DecodeHexAscii(grfData, width, height);
                grfData = Encoding.ASCII.GetBytes(decodedString);
                var enc = EncodeHexAscii(Encoding.ASCII.GetString(grfData));
            }

            //remove '\n'
            grfData = RemoveLineBreak(grfData);

            int currentBit = 0;
            int currentByte = 0;
            var binString = "";

            for (int h = 0; h < bitmapImage.Height; h++)
            {
                for (int w = 0; w < bitmapImage.Width * 2; w++)
                {
                    bitmapImage.SetPixel(w / 2, h, Color.White);

                    try
                    {
                        if (currentBit == 0)
                        {
                            var hex = ((char)grfData[currentByte]).ToString();
                            hex += ((char)grfData[currentByte + 1]).ToString();
                            binString = HexToBinaryString(hex);
                        }

                        if (binString.Substring(currentBit, 1) == "1")
                            bitmapImage.SetPixel(w / 2, h, Color.Black);

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
                if (var == 13 || var == 10)
                    continue;
                ret.Add(var);
            }
            return ret.ToArray();
        }
    }

}