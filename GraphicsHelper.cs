using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ZPLParser
{
    public class GraphicsHelper
    {
        /// <summary>
        /// https://github.com/mthurston/bitmap-to-zpl-csharp/blob/master/ZplConverterHelper.cs
        /// ZPLConverterHelper to assist in converting bitmap images into zpl strings.  Context of conversion output zpl strings will be either a string
        /// output ^GF[A] zpl command input or an entire valid ZPL label including headers and footers.
        /// </summary>
  
            // defaults black monochromatic threshold to 50%
            private int blackLimit = 380;
            private int total;
            private int widthBytes;
            private bool compressHex = false;

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
        /// Converts a Bitmap into a ZPL ^GF[A] command input.  Can also specify ZPL header and footer to allow easy printing of label.
        /// </summary>
        /// <param name="image">Bitmap containing image source for ^GF[A] command input string.</param>
        /// <param name="addHeaderAndFooter">if true surrounds the command input string with the ZPL headers and footers required to generate valid ZPL.</param>
        /// <returns>^GF[A] command input string</returns>
        public String ConvertFromImage(Bitmap image, Boolean addHeaderAndFooter=false)
        {
            String hexAscii = BitmapToString(image);
            if (compressHex)
            {
                hexAscii = EncodeHexAscii(hexAscii);
            }

            String zplCode = "^GFA," + total + "," + total + "," + widthBytes + ", " + hexAscii;

            if (addHeaderAndFooter)
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
        public String BitmapToString(Bitmap bitmapImage)
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
        public Bitmap StringToBitmap(string asciiHex, int height, int width)
        {
            //ASCII hexadecimal numbers, 2 digits per image byte CR and LF can be inserted
            var bytes = new List<byte>();
            var sp = asciiHex.Replace(((char)10).ToString(), "").Replace(((char)13).ToString(), "");
              
            //for (int i = 0; i < sp.Length-2; i+=2)
            //{
            //    var c1 = Convert.ToChar (sp.Substring(i, 1));
            //    var c2 = Convert.ToChar(sp.Substring(i+1, 1));
            //    var b = Convert.ToInt32((c1 << 4) + c2);
            //    bytes.AddRange(BitConverter.GetBytes(b).ToArray());
            //}
            return ByteToBitmap(bytes.ToArray(), height, width);
        }
        private Bitmap ByteToBitmap(byte[] data, int height, int width)
        {
            int w = width;
            int h = height;
            int ch = 1; //number of channels (ie. assuming 24 bit RGB in this case)

            byte[] imageData = new byte[w * h * ch]; //you image data here

            Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            BitmapData bmData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr pNative = bmData.Scan0; 
            Marshal.Copy(data, 0, pNative, w * h * ch);
            bitmap.UnlockBits(bmData);
              
            return bitmap;
                
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

        public void SetCompressHex(bool compressHex)
        {
            this.compressHex = compressHex;
        }

        /// <summary>
        /// Sets black pixel threshold for comparison of zpl pixels which determining whether to render or ignore pixels.  
        /// </summary>
        /// <param name="percentage">threshold percentage for comparison of pixels</param>
        /// <remarks>100+ percentage values will generate entirely black label.</remarks>
        public void SetBlacknessLimitPercentage(int percentage)
        {
            blackLimit = (percentage * 768 / 100);
        }
    } 
}
