using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
    class Parser
    {
        List<Token> tokens;
        List<Instruction> instructions = new List<Instruction>();
        int currentToken = 0;

        // Entry point
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            StatementList(instructions);

            if (HasMoreTokens())
                throw new SyntaxError("Tokens left after parsing completed, unexpected token " + PeekToken());
        }

        public List<Instruction> GetTree()
        {
            return instructions;
        }

        private void StatementList(List<Instruction> output)
        {
            bool statementParsed = Statement(output);

            if (HasMoreTokens() && statementParsed)
                StatementList(output);
        }

        private bool Statement(List<Instruction> output)
        {
            Token current = NextToken();

            Token.TokenType[] shortInstr = { Token.TokenType.UP, Token.TokenType.DOWN };
            Token.TokenType[] regInstr = { Token.TokenType.LEFT, Token.TokenType.RIGHT, Token.TokenType.FORW, Token.TokenType.BACK };

            // Handle short instructions, i.e. UP. and DOWN.
            if (shortInstr.Contains(current.type))
            {
                output.Add(HandleShort(current));
                Console.WriteLine("Parsed " + current.type + " instruction.");
                return true;
            }
            // Handle regular instructions, i.e. LEFT 2, FORW 10.
            else if (regInstr.Contains(current.type))
            {
                output.Add(HandleReg(current));
                Console.WriteLine("Parsed " + current.type + " instruction.");
                return true;
            }
            else if (current.type.Equals(Token.TokenType.COLOR))
            {
                output.Add(HandleColor(current));
                Console.WriteLine("Parsed " + current.type + " instruction.");
                return true;
            }
            else if (current.type.Equals(Token.TokenType.REP))
            {
                Console.WriteLine("Begin REP parsing");
                output.Add(HandleRep(current));
                Console.WriteLine("REP parsed");
                return true;
            }
            else if (current.type.Equals(Token.TokenType.WHITESPACE))
            {
                return true;
            }

            Console.WriteLine("Could not parse " + current.type + " instruction at index " + currentToken + ".");

            return false;
        }

        private Instruction HandleShort(Token current)
        {
            Token next = NextToken();

            if (next.type.Equals(Token.TokenType.WHITESPACE))
                next = NextToken();

            if (next.type.Equals(Token.TokenType.DOT))
                return new Instruction(current.type);
            else
                throw new SyntaxError(current.lineNum, "Expected DOT token after INSTR + whitespace, got " + next.type);
        }

        private Instruction HandleReg(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(Token.TokenType.WHITESPACE))
                throw new SyntaxError(current.lineNum, "Expected WHITESPACE token after INSTR, got " + next.type);

            next = NextToken();

            // Followed by a number
            if (!next.type.Equals(Token.TokenType.NUMBER))
                throw new SyntaxError(next.lineNum, "Expected number after whitespace token, got " + next.type);

            if (NextToken().type.Equals(Token.TokenType.DOT))
                return new Instruction(current.type, (int) next.num);
            else
                throw new SyntaxError(current.lineNum, "Expected DOT token at the end of instruction, got " + next.type);
        }

        private Instruction HandleColor(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(Token.TokenType.WHITESPACE))
                throw new SyntaxError(current.lineNum, "Expected whitespace after color token, got " + next.type);

            next = NextToken();

            // Followed by a number
            if (!next.type.Equals(Token.TokenType.HEX))
                throw new SyntaxError(next.lineNum, "Expected hex token after whitespace, got " + next.type);

            if (NextToken().type.Equals(Token.TokenType.DOT))
                return new Instruction(Token.TokenType.COLOR, next.hex);
            else
                throw new SyntaxError(current.lineNum, "Expected dot after hex token, got " + next.type);
        }

        private Instruction HandleRep(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(Token.TokenType.WHITESPACE))
                throw new SyntaxError(next.lineNum, "Whitespace expected after REP token, got " + next.type);

            next = NextToken();

            // Has to contain a number
            if (!next.type.Equals(Token.TokenType.NUMBER))
                throw new SyntaxError(next.lineNum, "Number expected after REP + whitespace, got " + next.type);

            int num = next.num;
            next = NextToken();

            if (!next.type.Equals(Token.TokenType.WHITESPACE))
                throw new SyntaxError(next.lineNum, "Whitespace expected after REP number, got " + next.type);

            // Get the token determining REP type
            var determinator = PeekToken();

            // Look at the determinator token to see whether one or multiple instructions
            // should be repeated
            if (determinator.type.Equals(Token.TokenType.QUOTE))
            {
                // Eat up the determinator quote
                NextToken();
                
                // Parse statements recursively
                // This will run until it encounters a quote token, once it does the Statement()
                // command will fail, thus kicking back here
                var recursiveList = new List<Instruction>();
                StatementList(recursiveList);

                // By now, other code will have swallowed our expected quote, so we have to peek back
                next = tokens.ElementAt(currentToken - 1);

                if (!next.type.Equals(Token.TokenType.QUOTE))
                    throw new SyntaxError(PeekToken().lineNum, "Quote expected after REP statements, got " + PeekToken().type);

                return new Instruction(Token.TokenType.REP, num, recursiveList);
                // recursion
            }
            else
            {
                List<Instruction> rep = new List<Instruction>();
                Statement(rep);
                return new Instruction(Token.TokenType.REP, num, rep);
            }
        }

        public bool HasMoreTokens()
        {
            return currentToken < tokens.Count;
        }

        public Token PeekToken()
        {
            if (!HasMoreTokens())
                throw new SyntaxError(tokens.Last().lineNum, "Tried to peek token when no more tokens exist");

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
