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

            TokenType[] shortInstr = { TokenType.UP, TokenType.DOWN };
            TokenType[] regInstr = { TokenType.LEFT, TokenType.RIGHT, TokenType.FORW, TokenType.BACK };

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
            else if (current.type.Equals(TokenType.COLOR))
            {
                output.Add(HandleColor(current));
                Console.WriteLine("Parsed " + current.type + " instruction.");
                return true;
            }
            else if (current.type.Equals(TokenType.REP))
            {
                Console.WriteLine("Begin REP parsing");
                output.Add(HandleRep(current));
                Console.WriteLine("REP parsed");
                return true;
            }
            else if (current.type.Equals(TokenType.WHITESPACE))
            {
                return true;
            }

            Console.WriteLine("Could not parse " + current.type + " instruction.");

            return false;
        }

        private Instruction HandleShort(Token current)
        {
            Token next = NextToken();

            if (next.type.Equals(TokenType.WHITESPACE))
                next = NextToken();

            if (next.type.Equals(TokenType.DOT))
                return new Instruction(current.type);
            else
                throw new SyntaxError(current.lineNum, "Expected DOT token after INSTR + whitespace, got " + next.type);
        }

        private Instruction HandleReg(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(current.lineNum, "Expected WHITESPACE token after INSTR, got " + next.type);

            next = NextToken();

            // Followed by a number
            if (!next.type.Equals(TokenType.NUMBER))
                throw new SyntaxError(next.lineNum, "Expected number after whitespace token, got " + next.type);

            if (NextToken().type.Equals(TokenType.DOT))
                return new Instruction(current.type, (int) next.num);
            else
                throw new SyntaxError(current.lineNum, "Expected DOT token at the end of instruction, got " + next.type);
        }

        private Instruction HandleColor(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(current.lineNum, "Expected whitespace after color token, got " + next.type);

            next = NextToken();

            // Followed by a number
            if (!next.type.Equals(TokenType.HEX))
                throw new SyntaxError(next.lineNum, "Expected hex token after whitespace, got " + next.type);

            if (NextToken().type.Equals(TokenType.DOT))
                return new Instruction(TokenType.COLOR, next.hex);
            else
                throw new SyntaxError(current.lineNum, "Expected dot after hex token, got " + next.type);
        }

        private Instruction HandleRep(Token current)
        {
            Token next = NextToken();

            // Has to contain a whitespace
            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(next.lineNum, "Whitespace expected after REP token");

            next = NextToken();

            // Has to contain a number
            if (!next.type.Equals(TokenType.NUMBER))
                throw new SyntaxError(next.lineNum, "Number expected after REP + whitespace");

            next = NextToken();

            if (!next.type.Equals(TokenType.WHITESPACE))
                throw new SyntaxError(next.lineNum, "Whitespace expected after REP number");

            int num = next.num;

            var determinator = PeekToken();

            // TODO: Handle 
            if (determinator.type.Equals(TokenType.QUOTE))
            {
                // As long as the 
                var recursiveList = new List<Instruction>();
                StatementList(recursiveList);

                if (!next.type.Equals(TokenType.QUOTE))
                    throw new SyntaxError(PeekToken().lineNum, "Quote expected after REP statements, got " + PeekToken().type);

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
