using System;

namespace S2
{
    class Token
	{
		public enum TokenType { UP, DOWN, LEFT, RIGHT, FORW, BACK, COLOR, REP, NUMBER, HEX, QUOTE, WHITESPACE, DOT }

		public TokenType type { get; private set; }
		public Object data { get; private set; }
        public int lineNum { get; private set; }

		public Token(int lineNum, TokenType type, int data)
		{
            this.lineNum = lineNum;
			this.type = type;
			this.data = data;
		}

        public Token(int lineNum, TokenType type, string hex)
        {
            this.lineNum = lineNum;
            this.type = type;
            data = data;
        }

		public Token(int lineNum, TokenType type)
		{
            this.lineNum = lineNum;
			this.type = type;
			data = null;
		}
	}
}
