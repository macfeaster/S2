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
        List<Instruction> instructions = new List<Instruction>();
        int repDepth = 0;
        int currentToken = 0;

        // how2entrypoint
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            StatementList(instructions);
        }

        private void StatementList(List<Instruction> output)
        {
            Statement(output);

            if (HasMoreTokens())
                StatementList(output);
        }

        private void Statement(List<Instruction> output)
        {
            Token current = NextToken();

            TokenType[] shortInstr = { TokenType.UP, TokenType.DOWN };
            TokenType[] regInstr = { TokenType.LEFT, TokenType.RIGHT, TokenType.FORW, TokenType.BACK };

            // Handle short instructions, i.e. UP. and DOWN.
            if (shortInstr.Contains(current.type))
            {
                output.Add(HandleShort(current));
            }
            // Handle regular instructions, i.e. LEFT 2, FORW 10.
            else if (regInstr.Contains(current.type))
            {
                output.Add(HandleReg(current));
            }
            else if (current.type.Equals(TokenType.COLOR))
            {
                output.Add(HandleColor(current));
            }
        }

        private Instruction HandleShort(Token current)
        {
            Token next = NextToken();

            if (next.type.Equals(TokenType.WHITESPACE))
                next = NextToken();

            if (next.type.Equals(TokenType.DOT))
                return new Instruction(current.type);
            else
                throw new SyntaxError(current.lineNum);
        }

        private Instruction HandleReg(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(current.lineNum);

            next = NextToken();

            // Followed by a number
            if (!next.type.Equals(TokenType.NUMBER))
                throw new SyntaxError(next.lineNum);

            if (NextToken().type.Equals(TokenType.DOT))
                return new Instruction(current.type, (int) next.num);
            else
                throw new SyntaxError(current.lineNum);
        }

        private Instruction HandleColor(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(current.lineNum);

            next = NextToken();

            // Followed by a number
            if (!next.type.Equals(TokenType.HEX))
                throw new SyntaxError(next.lineNum);

            if (NextToken().type.Equals(TokenType.DOT))
                return new Instruction(TokenType.COLOR, next.hex);
            else
                throw new SyntaxError(current.lineNum);
        }

        private Instruction HandleRep(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(next.lineNum);

            next = NextToken();

            // Has to contain a number
            if (!next.type.Equals(TokenType.NUMBER))
                throw new SyntaxError(next.lineNum);

            next = NextToken();

            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(next.lineNum);

            int num = next.num;

            var determinator = PeekToken();

            if (determinator.type.Equals(TokenType.QUOTE))
            {
                // As long as the 
                var recursiveList = new List<Instruction>();
                StatementList(recursiveList);
                return new Instruction(TokenType.REP, num, recursiveList);
                // recursion
            }
            else
            {
                List<Instruction> rep = new List<Instruction>();
                Statement(rep);
                return new Instruction(TokenType.REP, num, rep);
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
