namespace ZPLParser
{
    public static class Enums
    {
        public enum BlackWhite
        {
            B,
            W
        }

        public enum CharacterMode
        {
            N,
            A
        }

        public enum CompressionType
        {
            A,
            B,
            C
        }

        public enum DataInput
        {
            A,
            M
        }

        public enum DiagonalOrientation
        {
            R,
            L
        }

        public enum Direction
        {
            H,
            V,
            R
        }

        public enum ErrorCorrection
        {
            H,
            Q,
            M,
            L
        }

        public enum GraphicSymbolCharacter
        {
            RegisteredTradeMark,
            Copyright,
            TradeMark,
            UnderwritersLaboratoriesApproval,
            CanadianStandardsAssociationApproval
        }

        public enum Mode
        {
            N,
            U,
            A
        }

        public enum Orientation
        {
            N,
            R,
            I,
            B
        }

        public enum PrintMode
        {
            T, //Tear-off
            P, //Peel-off 
            R, //Rewind
            A, //Applicator
            C //Cutter
        }

        public enum TextJustification
        {
            L,
            C,
            R,
            J
        }

        public enum YesNo
        {
            Y,
            N
        }
    }
}