using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
	class Token
	{
		public enum TokenType { UP, DOWN, LEFT, RIGHT, FORW, BACK, COLOR, REP, NUMBER, HEX, QUOTE, DOT }

		private TokenType type { public get; private set; }
		private Object data { public get; private set; }

		public Token(TokenType type, int data)
		{
			this.type = type;
			this.data = data;
		}

		public Token(TokenType type)
		{
			this.type = type;
			this.data = null;
		}
	}
}
