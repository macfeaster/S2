﻿using System;

namespace S2
{
    class Token
	{
		public enum TokenType { UP, DOWN, LEFT, RIGHT, FORW, BACK, COLOR, REP, NUMBER, HEX, QUOTE, WHITESPACE, DOT }

		public TokenType type { get; private set; }
		public int num { get; private set; }
        public string hex { get; private set; }
        public int lineNum { get; private set; }

		public Token(int lineNum, TokenType type, int num)
		{
            this.lineNum = lineNum;
			this.type = type;
			this.num = num;
            hex = null;
		}

        public Token(int lineNum, TokenType type, string hex)
        {
            this.lineNum = lineNum;
            this.type = type;
            num = 0;
            this.hex = hex;
        }

		public Token(int lineNum, TokenType type)
		{
            this.lineNum = lineNum;
			this.type = type;
			num = 0;
            hex = null;
		}
	}
}
