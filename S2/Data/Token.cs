// Token.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

namespace S2
{
    /// <summary>
    /// Represents a Token the lexer picked up, with relevant metadata.
    /// </summary>
    internal class Token
    {
        public enum TokenType
        {
            Up, Down, Left, Right, Forw, Back, Color, Rep, Number, Hex, Quote, Whitespace, Dot, Invalid
        }

        public TokenType Type { get; private set; }
        public int Num { get; private set; }
        public string Hex { get; private set; }
        public int LineNum { get; private set; }

        /// <summary>
        /// Regular token, can contain a number if it is a Number token, which cannot be zero.
        /// </summary>
        public Token(int lineNum, TokenType type, int num)
        {
            LineNum = lineNum;
            Type = type;

            if (num != 0)
                Num = num;
            else
                throw new SyntaxError(lineNum, "Parameter value cannot be zero");
            Hex = null;
        }

        /// <summary>
        /// A Hex token, which contains a hex color string (7-character, e.g. #FF00FF)
        /// </summary>
        /// <param name="lineNum"></param>
        /// <param name="type"></param>
        /// <param name="hex"></param>
        public Token(int lineNum, TokenType type, string hex)
        {
            LineNum = lineNum;
            Type = type;
            Num = 0;
            Hex = hex;
        }

        /// <summary>
        /// Single token without any additional value.
        /// </summary>
        public Token(int lineNum, TokenType type)
        {
            LineNum = lineNum;
            Type = type;
            Num = 0;
            Hex = null;
        }

        public override string ToString()
        {
            return LineNum + ": " + Type;
        }
    }
}
