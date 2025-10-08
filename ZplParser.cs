using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Diagraph.Labelparser.ZPL
{
    public class ZplParser
    {
        public enum ElementCodeTypes
        {
            UNKNOWN,
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J,
            K,
            L,
            M,
            N,
            P,
            R,
            S,
            T,
            W,
            X,
            Z
        }

        private readonly List<FieldElement> _fieldelements = new List<FieldElement>();

        private byte[] _labelContentBytes;

        private BaseElement _parentElement;
        private Regex _regExElement;
        private Regex _regExLabelElement;
        private Regex _regExSimpleElement;

        public ZplParser()
        {
            Initialize();
        }

        public ZplParser(byte[] content)
        {
            Initialize();
            LabelContentBytes = content;
        }

        private char Start1 { get; } = '^';
        private char Start2 { get; } = '~';
        public List<BaseElement> Elements { get; private set; } = new List<BaseElement>();

        public List<Barcode1D> Barcodes1D
        {
            get { return Elements.Where(element => element.Base == typeof(Barcode1D)).OfType<Barcode1D>().ToList(); }
        }

        public List<Barcode2D> Barcodes2D
        {
            get { return Elements.Where(element => element.Base == typeof(Barcode2D)).OfType<Barcode2D>().ToList(); }
        }

        public List<GraphicElement> GraphicElements
        {
            get
            {
                var ret = new List<GraphicElement>();
                foreach (var item in Elements)
                {
                    var e = item as GraphicElement;
                    if (e != null) ret.Add(e);
                }

                return ret;
            }
        }

        public List<PositionedElement> PositionedElements
        {
            get
            {
                var ret = new List<PositionedElement>();
                foreach (var item in Elements)
                {
                    var e = item as PositionedElement;
                    if (e != null) ret.Add(e);
                }

                return ret;
            }
        }

        public List<FieldElement> FieldElements
        {
            get
            {
                return Elements.Where(element => element.Base == typeof(FieldElement)).OfType<FieldElement>().ToList();
            }
        }

        public Encoding ContentEncoding { get; set; }
        public string Error { get; set; }

        public byte[] LabelContentBytes
        {
            get => _labelContentBytes;
            set
            {
                _labelContentBytes = value;
                if (_labelContentBytes.Length <= 0)
                {
                    Error = "empty label";
                    return;
                }

                Elements.Clear();
                ParseLabelElements(0);
            }
        }

        /// <summary>
        ///     This method parse all elements of a label.
        /// </summary>
        /// <param name="startByteIndex"> This is the index where the first element starts. (After label properties)</param>
        public void ParseLabelElements(int startByteIndex)
        {
            // The byteCountElements is the whole size of all elements
            var byteCountElements = LabelContentBytes.Length - startByteIndex;

            // Start at the first  element to the end of the label. (i) will be increased by the value of each element.
            for (var i = 0; i < byteCountElements; i++)
                try
                {
                    // During the first run, labelElementsToParse contains the whole label elements as a string(starting from the second ';' to the end of the label).
                    // If an element is found, we increase i by the size of that element and look for the next element start code at i. 
                    var labelElementsToParse = ContentEncoding.GetString(LabelContentBytes, i + startByteIndex,
                        LabelContentBytes.Length - i - startByteIndex);

                    if (labelElementsToParse.Length >= 1 && (labelElementsToParse[0].Equals(Start1) ||
                                                             labelElementsToParse[0].Equals(Start2)))
                    {
                        var elementCode = tryToGetElementCodeType(labelElementsToParse[1]);
                        if (elementCode != ElementCodeTypes.UNKNOWN)
                            i += ParseAnElement(labelElementsToParse[0], elementCode, labelElementsToParse);
                    }
                }

                catch (Exception ex)
                {
                    Error = ex.Message;
                    //throw ex;
                }
        }

        private ElementCodeTypes tryToGetElementCodeType(char elementCodeCharacter1)
        {
            switch (elementCodeCharacter1)
            {
                case 'A':
                    return ElementCodeTypes.A;
                case 'B':
                    return ElementCodeTypes.B;
                case 'C':
                    return ElementCodeTypes.C;
                case 'D':
                    return ElementCodeTypes.D;
                case 'E':
                    return ElementCodeTypes.E;
                case 'F':
                    return ElementCodeTypes.F;
                case 'G':
                    return ElementCodeTypes.G;
                case 'H':
                    return ElementCodeTypes.H;
                case 'I':
                    return ElementCodeTypes.I;
                case 'J':
                    return ElementCodeTypes.J;
                case 'K':
                    return ElementCodeTypes.K;
                case 'L':
                    return ElementCodeTypes.L;
                case 'M':
                    return ElementCodeTypes.M;
                case 'N':
                    return ElementCodeTypes.N;
                case 'P':
                    return ElementCodeTypes.P;
                case 'R':
                    return ElementCodeTypes.R;
                case 'S':
                    return ElementCodeTypes.S;
                case 'T':
                    return ElementCodeTypes.T;
                case 'W':
                    return ElementCodeTypes.W;
                case 'X':
                    return ElementCodeTypes.X;
                case 'Z':
                    return ElementCodeTypes.Z;

                default:
                    return ElementCodeTypes.UNKNOWN;
            }
        }

        private void Initialize()
        {
            Elements = new List<BaseElement>();
            //printableElements = new List<BasePrintableElement>();
            ContentEncoding = Encoding.UTF8; // why? Encoding.GetEncoding(1252);

            //regExLabelGraphic = new Regex(
            //                              "(?<graphic>(\\w)+),(?<posx>(\\w)+),(?<posy>(\\w)+),(?<printDirection>(\\w)+),(?<datalength>(\\w)+),");

            //regExAuxPlusProperties = new Regex("(?<fonts>([\\w ]+,*)+)?;(?<properties>([\\w-,])+);");

            _regExLabelElement = new Regex("(?<tag>(\\w)+),(?<properties>([^;])+)(;(?<value>(.)+))*");
            _regExElement = new Regex("(?<tag>(\\w)+)");
            _regExElement = new Regex("(?<tag>)");
        }

        private int ParseAnElement(char start, ElementCodeTypes elementCode, string labelElementsToParse)
        {
            var anElement = "";
            for (var i = 0; i < labelElementsToParse.Length; i++)
            {
                if (i > 0 && (labelElementsToParse[i] == Start1 || labelElementsToParse[i] == Start2))
                    break;
                anElement += labelElementsToParse[i];
            }

            // allows us to split an element into properties and values parts. captures element code (tag), properties (until the first ';'), and value (entire remaining string)
            var contentLineMatch = _regExLabelElement.Match(anElement);
            if (!contentLineMatch.Success)
                contentLineMatch = _regExElement.Match(anElement);
            if (!contentLineMatch.Success)
                contentLineMatch = _regExSimpleElement.Match(anElement);

            var extra = 0;
            if (contentLineMatch.Success)
            {
                var tag = "" + start + elementCode;
                if (anElement.Length > tag.Length)
                    tag += anElement.Substring(2, 1);

                // the properties part is 
                var properties = contentLineMatch.Groups["properties"].Value;
                var value = contentLineMatch.Groups["value"].Value;
                var elementBytes = Encoding.UTF8.GetBytes(contentLineMatch.Value);
                if (value == "")
                {
                    value = anElement.Substring(tag.Length);
                    elementBytes = Encoding.UTF8.GetBytes(value);
                    properties = value;
                }

                switch (elementCode)
                {
                    case ElementCodeTypes.UNKNOWN:
                        break;
                    case ElementCodeTypes.A:
                        switch (tag)
                        {
                            case "^A0":
                            case "^A1":
                            case "^A2":
                            case "^A3":
                            case "^A4":
                            case "^A5":
                            case "^A6":
                            case "^A7":
                            case "^A8":
                            case "^A9":
                                //  ^A ScalableBitmappedFont
                                AddElement(new ScalableBitmappedFont(tag.Substring(2, 1), properties, elementBytes));
                                break;
                            case "^@":
                                //  ^A@
                                break;
                        }

                        break;
                    case ElementCodeTypes.B:
                        switch (tag)
                        {
                            case "^B1":
                                //  ^B1 Barcode Code11
                                break;
                            case "^B2":
                                //  ^B2 Barcode Interleaved 2 of 5
                                break;
                            case "^B3":
                                //  ^B3 Barcode Code 39
                                AddElement(new BarcodeCode39(properties, elementBytes));
                                break;
                            case "^B4":
                                //  ^B4 Barcode Code 49
                                break;
                            case "^B5":
                                //  ^B5  Barcode Planet Code
                                break;
                            case "^B7":
                                //  ^B7  Barcode PDF417
                                break;
                            case "^B8":
                                //  ^B8 Barcode EAN8
                                break;
                            case "^B9":
                                //  ^B9 Barcode UPC-E
                                break;
                            case "^BA":
                                //  ^BA Barcode Code 93
                                break;
                            case "^BB":
                                //  ^BB Barcode CODABLOCK
                                break;
                            case "^BC":
                                //  ^BC Barcode Code128
                                AddElement(new BarcodeCode128(properties, elementBytes));
                                break;
                            case "^BD":
                                //  ^BD Barcode UPS Maxicode
                                break;
                            case "^BE":
                                //  ^BE Barcode EAN13
                                break;
                            case "^BF":
                                //  ^BF Barcode Micro-PDF417
                                break;
                            case "^BI":
                                //  ^BI Barcode Industrial 2 of 5
                                break;
                            case "^BJ":
                                //  ^BJ Barcode Standard 2 of 5
                                break;
                            case "^BK":
                                //  ^BK Barcode ANSI Codabar
                                AddElement(new BarcodeAnsiCodabar(properties, elementBytes));
                                break;
                            case "^BL":
                                //  ^BL Barcode LOGMARS
                                break;
                            case "^BM":
                                //  ^BM Barcode MSI
                                break;
                            case "^BP":
                                //  ^BP Barcode Plessey
                                break;
                            case "^BQ":
                                //  ^BQ Barcode QR
                                AddElement(new BarcodeQR(properties, elementBytes));
                                break;
                            case "^BR":
                                //  ^BR Barcode RSS
                                break;
                            case "^BS":
                                //  ^BS Barcode UPC/EAN
                                break;
                            case "^BT":
                                //  ^BT Barcode TLC39
                                break;
                            case "^BU":
                                //  ^BU Barcode UPC-A
                                break;
                            case "^BX":
                                AddElement(new BarcodeDatamatrix(properties, elementBytes));
                                //  ^BX Barcode DataMatrix
                                break;
                            case "^BY":
                                //  ^BY Barcode FieldDefault
                                AddElement(new BarcodeFieldDefault(properties, elementBytes));
                                break;
                            case "^BZ":
                                //  ^BZ Barcode POSTNET
                                break;
                        }

                        break;
                    case ElementCodeTypes.C:
                        switch (tag)
                        {
                            case "^CD":
                                //  ^CD
                                break;
                            case "^CF":
                                //  ^CF 
                                var font = new ChangeDefaultFont(properties, elementBytes);

                                if (ScalableBitmappedFont.Current == null)
                                    ScalableBitmappedFont.Current = new ScalableBitmappedFont();

                                ScalableBitmappedFont.Current.FontHeight = font.CharacterHeight;
                                ScalableBitmappedFont.Current.FontWidth = font.CharacterWidth;
                                AddElement(font);
                                break;
                            case "^CI":
                                //  ^CI
                                break;
                            case "^CM":
                                //  ^CM
                                break;
                            case "^CO":
                                //  ^CO
                                break;
                            case "^CT":
                                //  ^CT
                                break;
                            case "^CV":
                                //  ^CV
                                break;
                            case "^CW":
                                //  ^CW
                                break;

                            case "~CC":
                                //  ~CC
                                break;
                            case "~CD":
                                //  ~CD
                                break;
                            case "~CT":
                                //  ~CT
                                break;
                        }

                        break;
                    case ElementCodeTypes.D:
                        switch (tag)
                        {
                            case "^DE":
                                //  ^DE
                                break;

                            case "~DB":
                                //  ~DB
                                break;
                            case "~DG":
                                //  ~DG Download Graphic
                                AddElement(new DownloadGraphic(properties, elementBytes), true);
                                break;
                            case "~DN":
                                //  ~DN Abort Download Graphic
                                break;
                            case "~DS":
                                //  ~DS
                                break;
                            case "~DT":
                                //  ~DT
                                break;
                            case "~DU":
                                //  ~DU
                                break;
                            case "~DY":
                                //  ~DY
                                break;
                        }

                        break;
                    case ElementCodeTypes.E:
                        switch (tag)
                        {
                            case "~EF":
                                //  ~EF
                                break;
                            case "~EG":
                                //  ~EG
                                break;
                        }

                        break;
                    case ElementCodeTypes.F:
                        switch (tag)
                        {
                            case "^FB":
                                //  ^FB                                
                                AddElement(new FieldBlock(properties, elementBytes), true);
                                break;
                            case "^FC":
                                //  ^FC FieldClock
                                AddElement(new FieldClock(properties, elementBytes), true);
                                break;
                            case "^FD":
                                //  ^FD FieldData
                                var fd = new FieldData(_parentElement, properties, elementBytes);
                                AddElement(fd, true);

                                if (ScalableBitmappedFont.Current == null)
                                    ScalableBitmappedFont.Current = new ScalableBitmappedFont();

                                var font = new ScalableBitmappedFont();
                                font.FontName = ScalableBitmappedFont.Current.FontName;
                                font.FontHeight = ScalableBitmappedFont.Current.FontHeight;
                                font.FontWidth = ScalableBitmappedFont.Current.FontWidth;

                                var tf = new TextField(FieldOrigin.Current.PositionX, FieldOrigin.Current.PositionY,
                                    fd.Data, font, NewLineConversionMethod.ToSpace, false,
                                    FieldReversePrint.Current != null);
                                AddElement(tf, true);
                                break;
                            case "^FH":
                                //  ^FH FieldHexadecimalIndicator
                                AddElement(new FieldHexadecimalIndicator(properties, elementBytes), true);
                                break;
                            case "^FM":
                                //  ^FM Multiple Field Origin Locations (PDF417 ^B7 and MicroPDF417 ^BF,true) 
                                break;
                            case "^FN":
                                //  ^FN
                                AddElement(new FieldNumber(properties, elementBytes), true);
                                break;
                            case "^FO":
                                //  ^FO
                                AddElement(new FieldOrigin(properties, elementBytes), true);
                                break;
                            case "^FP":
                                //  ^FP
                                AddElement(new FieldParameter(properties, elementBytes), true);
                                break;
                            case "^FR":
                                //  ^FR
                                AddElement(new FieldReversePrint(properties, elementBytes), true);
                                break;
                            case "^FS":
                                //  ^FS FieldSeparator
                                AddElement(new FieldSeparator(_parentElement, properties, elementBytes), true);

                                foreach (var item in PositionedElements)
                                    if (item.FieldElements == null)
                                    {
                                        item.FieldElements = new List<FieldElement>();
                                        foreach (var item2 in _fieldelements)
                                            item.FieldElements.Add(item2);
                                    }

                                _fieldelements.Clear();
                                break;
                            case "^FT":
                                //  ^FT 
                                AddElement(new FieldTypeset(properties, elementBytes), true);
                                break;
                            case "^FV":
                                //  ^FV
                                AddElement(new FieldVariable(properties, elementBytes), true);
                                break;
                            case "^FW":
                                //  ^FW
                                AddElement(new FieldOrientation(properties, elementBytes), true);
                                break;
                            case "^FX":
                                //  ^FX 
                                AddElement(new Comment(properties, elementBytes), true);
                                break;
                        }

                        break;
                    case ElementCodeTypes.G:
                        switch (tag)
                        {
                            case "^GB":
                                //  ^GB
                                AddElement(new GraphicBox(properties, elementBytes));
                                break;
                            case "^GC":
                                //  ^GC
                                AddElement(new GraphicCircle(properties, elementBytes));
                                break;
                            case "^GD":
                                //  ^GD
                                AddElement(new GraphicDiagonalLine(properties, elementBytes));
                                break;
                            case "^GE":
                                //  ^GE
                                AddElement(new GraphicEllipse(properties, elementBytes));
                                break;
                            case "^GF":
                                //  ^GF 
                                AddElement(new GraphicField(properties, elementBytes));
                                break;
                            case "^GS":
                                //  ^GS
                                AddElement(new GraphicSymbol(properties, elementBytes));
                                break;
                        }

                        break;
                    case ElementCodeTypes.H:
                        switch (tag)
                        {
                            case "^HF":
                                //  ^HF
                                break;
                            case "^HG":
                                //  ^HG
                                break;
                            case "^HH":
                                //  ^HH
                                break;
                            case "^HW":
                                //  ^HW
                                break;
                            case "^HY":
                                //  ^Y
                                break;
                            case "^HZ":
                                //  ^Z
                                break;

                            case "~HB":
                                //  ~HB
                                break;
                            case "~HD":
                                //  ~HD
                                break;
                            case "~HI":
                                //  ~HI
                                break;
                            case "~HM":
                                //  ~HM
                                break;
                            case "~HS":
                                //  ~HS
                                break;
                            case "~HU":
                                //  ~HU
                                break;
                        }

                        break;
                    case ElementCodeTypes.I:
                        switch (tag)
                        {
                            case "^ID":
                                //  ^ID
                                break;
                            case "^IL":
                                //  ^IL
                                break;
                            case "^IM":
                                //  ^IM
                                break;
                            case "^IS":
                                //  ^IS
                                break;
                        }

                        break;
                    case ElementCodeTypes.J:
                        switch (tag)
                        {
                            case "^JB":
                                //  ^JB
                                break;
                            case "^JI":
                                //  ~JI
                                break;
                            case "^JJ":
                                //  ~JJ
                                break;
                            case "^JM":
                                //  ~JM
                                break;
                            case "^JS":
                                //  ~JS
                                break;
                            case "^JT":
                                //  ~JT
                                break;
                            case "^JW":
                                //  ~JW
                                break;
                            case "^JZ":
                                //  ~JZ
                                break;

                            case "~JA":
                                //  ~JA
                                break;
                            case "~JB":
                                //  ~JB
                                break;
                            case "~JC":
                                //  ~JC
                                break;
                            case "~JD":
                                //  ~JD
                                break;
                            case "~JE":
                                //  ~JE
                                break;
                            case "~JF":
                                //  ~JF
                                break;
                            case "~JG":
                                //  ~JG
                                break;
                            case "~JI":
                                //  ~JI
                                break;
                            case "~JL":
                                //  ~JL
                                break;
                            case "~JN":
                                //  ~JN
                                break;
                            case "~JO":
                                //  ~JO
                                break;
                            case "~JP":
                                //  ~JP
                                break;
                            case "~JQ":
                                //  ~JQ
                                break;
                            case "~JR":
                                //  ~JR
                                break;
                            case "~JS":
                                //  ~JS
                                break;
                            case "~JU":
                                //  ~JU
                                break;
                            case "~JX":
                                //  ~JX
                                break;
                        }

                        break;
                    case ElementCodeTypes.K:
                        switch (tag)
                        {
                            case "^KD":
                                //  ^KD
                                break;
                            case "^KL":
                                //  ^KL
                                break;
                            case "^KN":
                                //  ^KN
                                break;
                            case "^KP":
                                //  ^KP
                                break;

                            case "~KB":
                                //  ~KB
                                break;
                        }

                        break;
                    case ElementCodeTypes.L:
                        switch (tag)
                        {
                            case "^LH":
                                //  ^LH  ^LH0,0
                                AddElement(new LabelHome(properties, elementBytes));
                                break;
                            case "^LL":
                                //  ^LL LabelLength 
                                AddElement(new LabelLength(properties, elementBytes));
                                break;
                            case "^LR":
                                //  ^LR 
                                AddElement(new LabelReverse(properties, elementBytes));
                                break;
                            case "^LS":
                                //  ^LS LabelShfit   
                                AddElement(new LabelShfit(properties, elementBytes));
                                break;
                            case "^LT":
                                //  ^LT
                                AddElement(new LabelTop(properties, elementBytes));
                                break;
                        }

                        break;
                    case ElementCodeTypes.M:
                        switch (tag)
                        {
                            case "^MC":
                                //  ^MC
                                break;
                            case "^MD":
                                //  ^MD
                                break;
                            case "^MF":
                                //  ^MF
                                break;
                            case "^ML":
                                //  ^ML
                                break;
                            case "^MM":
                                //  ^MM PrintMode
                                AddElement(new PrintMode(properties, elementBytes));
                                break;
                            case "^MN":
                                //  ^MN
                                break;
                            case "^MP":
                                //  ^MP
                                break;
                            case "^MT":
                                //  ^MT
                                break;
                            case "^MU":
                                //  ^MU
                                break;
                            case "^MW":
                                //  ^MW
                                break;
                        }

                        break;
                    case ElementCodeTypes.N:
                        switch (tag)
                        {
                            case "~NC":
                                //  ~NC
                                break;
                            case "^NI":
                                //  ^NI
                                break;
                            case "~NR":
                                //  ~NI
                                break;
                            case "^NS":
                                //  ^NS
                                break;
                            case "~NT":
                                //  ~NT
                                break;
                        }

                        break;
                    case ElementCodeTypes.P:
                        switch (tag)
                        {
                            case "^PF":
                                //  ^PF
                                break;
                            case "~PF":
                                //  ~PF
                                break;
                            case "^PM":
                                //  ^PM
                                break;
                            case "^PO":
                                //  ^PO
                                break;
                            case "^PP":
                                //  ^PP
                                break;
                            case "~PP":
                                //  ~PP
                                break;
                            case "^PQ":
                                //  ^PQ
                                AddElement(new PrintQuantity(properties, elementBytes));
                                break;
                            case "^PR":
                                //  ^PR
                                break;
                            case "~PR":
                                //  ~PR
                                break;
                            case "~PS":
                                //  ~PS
                                break;
                            case "^PW":
                                //  ^PW
                                AddElement(new PrintWidth(properties, elementBytes));
                                break;
                        }

                        break;
                    case ElementCodeTypes.R:
                        switch (tag)
                        {
                            case "~RO":
                                //  ~RO
                                break;
                        }

                        break;
                    case ElementCodeTypes.S:
                        switch (tag)
                        {
                            case "^SC":
                                //  ^SC
                                break;
                            case "~SD":
                                //  ~SD
                                break;
                            case "^SE":
                                //  ^SE
                                break;
                            case "^SF":
                                //  ^SF
                                break;
                            case "^SL":
                                //  ^SL
                                break;
                            case "^SN":
                                //  ^SN
                                break;
                            case "^SO":
                                //  ^SO
                                break;
                            case "^SP":
                                //  ^SP
                                break;
                            case "^SQ":
                                //  ^SQ
                                break;
                            case "^SR":
                                //  ^SR
                                break;
                            case "^SS":
                                //  ^SS
                                break;
                            case "^ST":
                                //  ^ST
                                break;
                            case "^SX":
                                //  ^SX
                                break;
                            case "^SZ":
                                //  ^SZ
                                break;
                        }

                        break;
                    case ElementCodeTypes.T:
                        switch (tag)
                        {
                            case "~TA":
                                //  ~TA
                                break;
                            case "^TO":
                                //  ^TO
                                break;
                        }

                        break;
                    case ElementCodeTypes.W:
                        switch (tag)
                        {
                            case "~WC":
                                //  ~WC
                                break;
                            case "^WD":
                                //  ^WD
                                break;
                        }

                        break;
                    case ElementCodeTypes.X:
                        switch (tag)
                        {
                            case "^XA":
                                //  ^XA
                                AddElement(new StartFormat(properties, elementBytes));
                                break;
                            case "^XB":
                                //  ^XB
                                break;
                            case "^XF":
                                //  ^XF
                                break;
                            case "^XG":
                                //  ^XG
                                AddElement(new RecallGraphic(properties, elementBytes));
                                break;
                            case "^XZ":
                                AddElement(new EndFormat(properties, elementBytes));
                                //  ^XZ
                                break;
                        }

                        break;
                    case ElementCodeTypes.Z:
                        switch (tag)
                        {
                            case "^ZZ":
                                //  ^ZZ
                                break;
                        }

                        break;
                }
            }

            var length = contentLineMatch.Length + extra;
            return length;
        }

        private void AddElement(BaseElement element, bool fieldElement = false)
        {
            try
            {
                Elements.Add(element);

                if (_parentElement != null && _parentElement.HasChild)
                {
                    _parentElement.Child = element;
                    element.Parent = _parentElement;
                }

                if (element.HasChild)
                    _parentElement = element;
                else
                    _parentElement = null;

                if (fieldElement)
                    _fieldelements.Add((FieldElement)element);
            }
            catch (Exception ex)
            {
            }
        }

        public FieldElement GetElement(List<FieldElement> fieldElements, Type type)
        {
            if (fieldElements != null)
                foreach (var item in fieldElements)
                    if (item.GetType() == type)
                        return item;

            return null;
        }

        public string GetTree()
        {
            var sb = new StringBuilder();
            foreach (var element in Elements)
            {
                var elem = element.Parent;
                while (elem != null)
                {
                    sb.Append("|    ");
                    elem = elem.Parent;
                }

                var line = "|---" + element.ToString().Replace("ZPLParser.", "") + "  " + element.RenderToString();
                sb.AppendLine(line);
            }

            return sb.ToString();
        }

        public byte[] Rebuild()
        {
            var ms = new MemoryStream();
            foreach (var element in Elements)
            {
                var rendered = element.Render(ZPLRenderOptions.DefaultOptions);

                foreach (var line in rendered)
                {
                    var bytes = Encoding.UTF8.GetBytes(line);
                    ms.Write(bytes, 0, bytes.Length);
                    bytes = Encoding.UTF8.GetBytes(Environment.NewLine);
                    ms.Write(bytes, 0, bytes.Length);
                }
            }

            return ms.ToArray();
        }

        private string RenderTabs(BaseElement fieldData)
        {
            var ret = "";
            var tab = (char)9;
            var elem = fieldData.Child;
            while (elem != null && elem.HasChild)
            {
                ret += tab;
                elem = elem.Child;
            }

            return ret;
        }
    }
}