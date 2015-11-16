using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static S2.Token;

namespace S2
{
    class Parser
    {
        List<Token> tokens;
        int repDepth = 0;
        int currentToken = 0;

        // how2entrypoint
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private void StatementList()
        {
            Statement();

            if (HasMoreTokens())
                StatementList();
        }

        private void Statement()
        {
            Token current = NextToken();

            TokenType[] shortInstr = { TokenType.UP, TokenType.DOWN };
            TokenType[] regInstr = { TokenType.LEFT, TokenType.RIGHT, TokenType.FORW, TokenType.BACK };

            // Handle short instructions, i.e. UP. and DOWN.
            if (shortInstr.Contains(current.type))
            {
                Token next = NextToken();

                if (next.type.Equals(TokenType.WHITESPACE))
                    next = NextToken();

                if (next.type.Equals(TokenType.DOT))

            }
        }

        public bool HasMoreTokens()
        {
            return currentToken < tokens.Count;
        }

        public Token PeekToken()
        {
            if (!HasMoreTokens())
                throw new SyntaxError(tokens.Last().lineNum);

            return tokens.ElementAt(currentToken);
        }

        public Token NextToken()
        {
            Token t = PeekToken();
            currentToken++;
            return t;
        }

    }
}
