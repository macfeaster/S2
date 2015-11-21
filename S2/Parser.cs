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
            {
                StatementList(output);
            }
            else
            {
                Log.Debug("HasMoreTokens: " + HasMoreTokens());
                Log.Debug("currentToken: " + currentToken);
                Log.Debug("NumTokens: " + tokens.Count);
                Log.Debug("StatementParsed: " + statementParsed);
                Log.Debug("Aborting parsing");
            }
        }

        private bool Statement(List<Instruction> output)
        {
            Token current = NextToken();

            Token.TokenType[] shortInstr = { Token.TokenType.UP, Token.TokenType.DOWN };
            Token.TokenType[] regInstr = { Token.TokenType.LEFT, Token.TokenType.RIGHT, Token.TokenType.FORW, Token.TokenType.BACK };

            Log.Debug("Beginning parse of " + current);

            // Handle short instructions, i.e. UP. and DOWN.
            if (shortInstr.Contains(current.type))
            {
                Log.Debug("--> Called " + current.type + " parser.");
                output.Add(HandleShort(current));
                Log.Debug("--> Parsed " + current.type + " instruction.");
                return true;
            }
            // Handle regular instructions, i.e. LEFT 2, FORW 10.
            else if (regInstr.Contains(current.type))
            {
                Log.Debug("--> Called " + current.type + " parser.");
                output.Add(HandleReg(current));
                Log.Debug("--> Parsed " + current.type + " instruction.");
                return true;
            }
            // Handle COLOR instructions
            else if (current.type.Equals(Token.TokenType.COLOR))
            {
                Log.Debug("--> Called " + current.type + " parser.");
                output.Add(HandleColor(current));
                Log.Debug("--> Parsed " + current.type + " instruction.");
                return true;
            }
            // Handle REP instructions
            else if (current.type.Equals(Token.TokenType.REP))
            {
                Log.Debug("--> Begin REP parsing");
                output.Add(HandleRep(current));
                Log.Debug("--> REP parsed");
                return true;
            }
            // Dismiss whitespace by telling StatementList we parsed something when
            // all we really do is move the Token pointer forward one step
            else if (current.type.Equals(Token.TokenType.WHITESPACE))
            {
                return true;
            }
            else if (current.type.Equals(Token.TokenType.INVALID))
            {
                throw new SyntaxError(current.lineNum, "Encountered illegal token");
            }
            else
            {
                Log.Debug("Could not parse " + current.type + " instruction at index " + currentToken + ".");
                return false;
            }
        }

        private Instruction HandleShort(Token current)
        {
            Token next = NextToken();

            Expect(Token.TokenType.DOT, " at end of instruction", ref next);
            return new Instruction(current.type);
        }

        private Instruction HandleReg(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.WHITESPACE, "after INSTR", ref next);

            // Followed by a number
            Expect(Token.TokenType.NUMBER, "after whitespace token", ref next);
            int num = next.num;

            next = NextToken();

            Expect(Token.TokenType.DOT, " at end of instruction", ref next);
            return new Instruction(current.type, num);
        }

        private Instruction HandleColor(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.WHITESPACE, "after color token", ref next);

            // Followed by a hex color code
            Expect(Token.TokenType.HEX, "after whitespace", ref next);
            string hex = next.hex;

            Expect(Token.TokenType.DOT, " at end of instruction", ref next);
            return new Instruction(Token.TokenType.COLOR, hex);
        }

        private Instruction HandleRep(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.WHITESPACE, "after REP token", ref next);

            // Followed by a number, which is saved
            Expect(Token.TokenType.NUMBER, "after REP + whitespace", ref next);
            int num = next.num;

            // Followed by whitespace again
            Expect(Token.TokenType.WHITESPACE, "after REP number", ref next);

            // Get the token determining REP type
            var determinator = PeekToken();

            while (determinator.type.Equals(Token.TokenType.WHITESPACE))
            {
                Log.Debug("Ignored whitespace token " + determinator);
                determinator = PeekToken();
                if (determinator.type.Equals(Token.TokenType.WHITESPACE))
                    determinator = NextToken();
            }

            Log.Debug("Determinator: " + determinator.ToString());

            // Look at the determinator token to see whether one or multiple instructions
            // should be repeated
            if (determinator.type.Equals(Token.TokenType.QUOTE))
            {
                // Eat up the determinator quote
                NextToken();
                
                // Parse statements recursively
                // This will run until it encounters a lone quote token (not part of a REP start),
                // once it does the Statement(), command will fail, thus kicking back here
                var recursiveList = new List<Instruction>();
                Log.Debug("----> Begin REP recursion ...");
                StatementList(recursiveList);
                Log.Debug("----> Ended REP recursion, parsed " + recursiveList.Count + " statements");

                // By now, other code will have swallowed our expected quote, so we have to peek back
                next = tokens.ElementAt(currentToken - 1);

                if (!next.type.Equals(Token.TokenType.QUOTE))
                    throw new SyntaxError(PeekToken().lineNum, "Quote expected after REP statements, got " + PeekToken().type);

                return new Instruction(Token.TokenType.REP, num, recursiveList);
            }
            else
            {
                // To repeat a single instruction, just make a single statement parse
                List<Instruction> rep = new List<Instruction>();
                Statement(rep);
                return new Instruction(Token.TokenType.REP, num, rep);
            }
        }

        /// <summary>
        /// Expect a token to be next in line, throwing a SyntaxError otherwise
        /// </summary>
        /// <param name="type">Token type to expect</param>
        /// <param name="message">Error information, in the format of "Expected Y, {message}, got Z"</param>
        /// <param name="next">A reference to the Token object used for forward-lookup</param>
        public void Expect(Token.TokenType type, string message, ref Token next)
        {
            next = NextToken();

            // If we're not specifically expecting whitespace, we can safely ignore any such
            if (!type.Equals(Token.TokenType.WHITESPACE))
            {
                while (next.type.Equals(Token.TokenType.WHITESPACE))
                {
                    Log.Debug("---> Not expecting whitespace, expecting " + type + ", ignored whitespace token " + next + " at line " + next.lineNum);
                    next = NextToken();
                }
            }

            if (!next.type.Equals(type))
                throw new SyntaxError(next.lineNum, "Expected " + type + " " + message + ", got " + next.type + " at line " + next.lineNum);
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
