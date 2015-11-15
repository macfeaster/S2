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

		public TokenType type { get; private set; }
		public Object data { get; private set; }

		public Token(TokenType type, int data)
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
