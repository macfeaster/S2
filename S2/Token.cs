using System;

namespace S2
{
    class Token
	{
		public enum TokenType { UP, DOWN, LEFT, RIGHT, FORW, BACK, COLOR, REP, NUMBER, HEX, QUOTE, WHITESPACE, DOT }

		public TokenType type { get; private set; }
		public Object data { get; private set; }

		public Token(TokenType type, int data)
		{
			this.type = type;
			this.data = data;
		}

        public Token(TokenType type, string hex)
        {
            this.type = type;
            this.data = data;
        }

		public Token(TokenType type)
		{
			this.type = type;
			data = null;
		}
	}
}
