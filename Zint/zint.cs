#region

using System;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Diagraph.Labelparser.ZPL.ZintSupport;

public static class Zint
{
    public enum ECI
    {
        ISO88591_LatinalphabetNo1default = 3,
        ISO88592_LatinalphabetNo2 = 4,
        ISO88593_LatinalphabetNo3 = 5,
        ISO88594_LatinalphabetNo4 = 6,
        ISO88595_LatinCyrillicalphabet = 7,
        ISO88596_LatinArabicalphabet = 8,
        ISO88597_LatinGreekalphabet = 9,
        ISO88598_LatinHebrewalphabet = 10,
        ISO88599_LatinalphabetNo5 = 11,
        ISO885910_LatinalphabetNo6 = 12,
        ISO885911_LatinThaialphabet = 13,
        ISO885913_LatinalphabetNo7 = 15,
        ISO885914_LatinalphabetNo8Celtic = 16,
        ISO885915_LatinalphabetNo9 = 17,
        ISO885916_LatinalphabetNo10 = 18,
        ShiftJisJISX0208andJISX0201 = 20,
        Windows1250_Latin2CentralEurope = 21,
        Windows1251_Cyrillic = 22,
        Windows1252_Latin1 = 23,
        Windows1256_Arabic = 24,
        UCS2UnicodeHighOrderByteFirst = 25,
        Unicode_UTF8 = 26,
        ISO6461991_7bitCharset = 27,
        Big5TaiwanChineseCharset = 28,
        GBPRCChineseCharset = 29,
        KoreanCharsetKSX10011998 = 30
    }

    public static bool LinuxOS => Path.DirectorySeparatorChar == '/';

    [DllImport(@"zint\zint.dll", EntryPoint = "ZBarcode_Create", ExactSpelling = false,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ZBarcode_Create_Windows();

    [DllImport(@"zint\libzint.so", EntryPoint = "ZBarcode_Create", ExactSpelling = false,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr ZBarcode_Create_Linux();

    [DllImport(@"zint\zint.dll", EntryPoint = "ZBarcode_Encode_and_Buffer", ExactSpelling = false,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int ZBarcode_Encode_and_Buffer_Windows(ref zint_symbol symbol, IntPtr input, int length,
        int rotateAngle);

    [DllImport(@"zint\libzint.so", EntryPoint = "ZBarcode_Encode_and_Buffer", ExactSpelling = false,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern int ZBarcode_Encode_and_Buffer_Linux(ref zint_symbol symbol, IntPtr input, int length,
        int rotateAngle);

    [DllImport(@"zint\zint.dll", EntryPoint = "ZBarcode_Delete", ExactSpelling = false,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern void ZBarcode_Delete_Windows(ref zint_symbol symbol);

    [DllImport(@"zint\libzint.so", EntryPoint = "ZBarcode_Delete", ExactSpelling = false,
        CallingConvention = CallingConvention.Cdecl)]
    private static extern void ZBarcode_Delete_Linux(ref zint_symbol symbol);

    internal static IntPtr Create()
    {
        return LinuxOS ? ZBarcode_Create_Linux() : ZBarcode_Create_Windows();
    }

    internal static int EncodeAndBuffer(ref zint_symbol symbol, IntPtr pointer, int encBytesLength, int angle)
    {
        return LinuxOS
            ? ZBarcode_Encode_and_Buffer_Linux(ref symbol, pointer, encBytesLength, angle)
            : ZBarcode_Encode_and_Buffer_Windows(ref symbol, pointer, encBytesLength, angle);
    }

    internal static void Delete(ref zint_symbol symbol)
    {
        if (LinuxOS)
            ZBarcode_Delete_Linux(ref symbol);
        else
            ZBarcode_Delete_Windows(ref symbol);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct zint_symbol
    {
        public int symbology;
        public int height;
        public int whitespace_width;
        public int border_width;
        public int output_options;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string fgcolour;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string bgcolour;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string outfile;

        public float scale;
        public int option_1;
        public int option_2;
        public int option_3;
        public int show_hrt;
        public int fontsize;
        public int input_mode;
        public int eci;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string text;

        public int rows;
        public int width;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string primary;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28600)]
        public string encoded_data;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200, ArraySubType = UnmanagedType.I4)]
        public int[] row_height;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string errtxt;

        public IntPtr bitmap;
        public int bitmap_width;
        public int bitmap_height;
        public uint bitmap_byte_length;
        public float dot_size;
        public IntPtr rendered;
        public int debug;
    }
}

public struct InputMode
{
    public static int DATA_MODE = 0;
    public static int UNICODE_MODE = 1;
    public static int GS1_MODE = 2;
    public static int KANJI_MODE = 3;
    public static int SJIS_MODE = 4;
}

public struct ErrorCode
{
    public static int WARN_INVALID_OPTION = 2;
    public static int ERROR_TOO_LONG = 5;
    public static int ERROR_INVALID_DATA = 6;
    public static int ERROR_INVALID_CHECK = 7;
    public static int ERROR_INVALID_OPTION = 8;
    public static int ERROR_ENCODING_PROBLEM = 9;
    public static int ERROR_FILE_ACCESS = 10;
    public static int ERROR_MEMORY = 11;
}

public struct BarcodeType
{
    public static int BARCODE_EXCODE39 = 9;
    public static int BARCODE_EANX = 13;
    public static int BARCODE_EANX_CHK = 14;
    public static int BARCODE_EAN128 = 16;
    public static int BARCODE_CODABAR = 18;
    public static int BARCODE_CODE128 = 20;
    public static int BARCODE_DPLEIT = 21;
    public static int BARCODE_DPIDENT = 22;
    public static int BARCODE_CODE16K = 23;
    public static int BARCODE_CODE49 = 24;
    public static int BARCODE_CODE93 = 25;
    public static int BARCODE_FLAT = 28;
    public static int BARCODE_RSS14 = 29;
    public static int _BARCODE_RSS14 = -29;
    public static int BARCODE_RSS_LTD = 30;
    public static int BARCODE_RSS_EXP = 31;
    public static int BARCODE_TELEPEN = 32;
    public static int BARCODE_UPCA = 34;
    public static int BARCODE_UPCA_CHK = 35;
    public static int BARCODE_UPCE = 37;
    public static int BARCODE_UPCE_CHK = 38;
    public static int BARCODE_POSTNET = 40;
    public static int BARCODE_MSI_PLESSEY = 47;
    public static int BARCODE_FIM = 49;
    public static int BARCODE_LOGMARS = 50;
    public static int BARCODE_PHARMA = 51;
    public static int BARCODE_PZN = 52;
    public static int BARCODE_PHARMA_TWO = 53;
    public static int BARCODE_PDF417 = 55;
    public static int BARCODE_PDF417TRUNC = 56;
    public static int BARCODE_MAXICODE = 57;
    public static int BARCODE_QRCODE = 58;
    public static int BARCODE_CODE128B = 60;
    public static int BARCODE_AUSPOST = 63;
    public static int BARCODE_AUSREPLY = 66;
    public static int BARCODE_AUSROUTE = 67;
    public static int BARCODE_AUSREDIRECT = 68;
    public static int BARCODE_ISBNX = 69;
    public static int BARCODE_RM4SCC = 70;
    public static int BARCODE_DATAMATRIX = 71;
    public static int BARCODE_EAN14 = 72;
    public static int BARCODE_CODABLOCKF = 74;
    public static int BARCODE_NVE18 = 75;
    public static int BARCODE_JAPANPOST = 76;
    public static int BARCODE_KOREAPOST = 77;
    public static int BARCODE_RSS14STACK = 79;
    public static int BARCODE_RSS14STACK_OMNI = 80;
    public static int BARCODE_RSS_EXPSTACK = 81;
    public static int BARCODE_PLANET = 82;
    public static int BARCODE_MICROPDF417 = 84;
    public static int BARCODE_ONECODE = 85;
    public static int BARCODE_PLESSEY = 86;
    public static int BARCODE_TELEPEN_NUM = 87;
    public static int BARCODE_ITF14 = 89;
    public static int BARCODE_KIX = 90;
    public static int BARCODE_AZTEC = 92;
    public static int BARCODE_DAFT = 93;
    public static int BARCODE_MICROQR = 97;
    public static int BARCODE_HIBC_128 = 98;
    public static int BARCODE_HIBC_39 = 99;
    public static int BARCODE_HIBC_DM = 102;
    public static int BARCODE_HIBC_QR = 104;
    public static int BARCODE_HIBC_PDF = 106;
    public static int BARCODE_HIBC_MICPDF = 108;
    public static int BARCODE_HIBC_BLOCKF = 110;
    public static int BARCODE_HIBC_AZTEC = 112;
    public static int BARCODE_DOTCODE = 115;
    public static int BARCODE_HANXIN = 116;
    public static int BARCODE_AZRUNE = 128;
    public static int BARCODE_CODE32 = 129;
    public static int BARCODE_EANX_CC = 130;
    public static int BARCODE_EAN128_CC = 131;
    public static int BARCODE_RSS14_CC = 132;
    public static int BARCODE_RSS_LTD_CC = 133;
    public static int BARCODE_RSS_EXP_CC = 134;
    public static int BARCODE_UPCA_CC = 135;
    public static int BARCODE_UPCE_CC = 136;
    public static int BARCODE_RSS14STACK_CC = 137;
    public static int BARCODE_RSS14_OMNI_CC = 138;
    public static int BARCODE_RSS_EXPSTACK_CC = 139;
    public static int BARCODE_CHANNEL = 140;
}
