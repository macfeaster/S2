using System.Collections.Generic;
using System.Linq;

namespace S2
{
    class Parser
    {
        List<Token> tokens;
        List<Instruction> instructions = new List<Instruction>();
        private int _currentToken;

        // Entry point
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;

            if (!HasMoreTokens())
                throw new SyntaxError(1, "No tokens to parse");

            StatementList(instructions, false);

            if (HasMoreTokens())
                throw new SyntaxError(tokens.Last().LineNum, "Tokens left after parsing completed, unexpected token " + PeekToken());
        }

        public List<Instruction> GetTree()
        {
            return instructions;
        }

        private void StatementList(List<Instruction> output, bool inRecursion)
        {
            var statementParsed = Statement(output, inRecursion);

            if (HasMoreTokens() && statementParsed)
            {
                StatementList(output, inRecursion);
            }
            else
            {
                Log.Debug("HasMoreTokens: " + HasMoreTokens());
                Log.Debug("_currentToken: " + _currentToken);
                Log.Debug("NumTokens: " + tokens.Count);
                Log.Debug("StatementParsed: " + statementParsed);
                Log.Debug("Aborting parsing");
            }
        }

        private bool Statement(List<Instruction> output, bool inRecursion)
        {
            var current = NextToken();

            Token.TokenType[] shortInstr = { Token.TokenType.Up, Token.TokenType.Down };
            Token.TokenType[] regInstr = { Token.TokenType.Left, Token.TokenType.Right, Token.TokenType.Forw, Token.TokenType.Back };
            Token.TokenType[] illegalInstr = { Token.TokenType.Invalid, Token.TokenType.Number, Token.TokenType.Hex, Token.TokenType.Dot };

            Log.Debug("Beginning parse of " + current);

            // SyntaxError when encountering an illegal token that does not belong here
            if (illegalInstr.Contains(current.Type))
            {
                throw new SyntaxError(current.LineNum, "Encountered illegal token");
            }
            // Handle short instructions, i.e. Up. and Down.
            else if (shortInstr.Contains(current.Type))
            {
                Log.Debug("--> Called " + current.Type + " parser.");
                output.Add(HandleShort(current));
                Log.Debug("--> Parsed " + current.Type + " instruction.");
                return true;
            }
            // Handle regular instructions, i.e. Left 2, Forw 10.
            else if (regInstr.Contains(current.Type))
            {
                Log.Debug("--> Called " + current.Type + " parser.");
                output.Add(HandleReg(current));
                Log.Debug("--> Parsed " + current.Type + " instruction.");
                return true;
            }
            // Handle Color instructions
            else if (current.Type.Equals(Token.TokenType.Color))
            {
                Log.Debug("--> Called " + current.Type + " parser.");
                output.Add(HandleColor(current));
                Log.Debug("--> Parsed " + current.Type + " instruction.");
                return true;
            }
            // Handle Rep instructions
            else if (current.Type.Equals(Token.TokenType.Rep))
            {
                Log.Debug("--> Begin Rep parsing");
                output.Add(HandleRep());
                Log.Debug("--> Rep parsed");
                return true;
            }
            // Dismiss whitespace by telling StatementList we parsed something when
            // all we really do is move the Token pointer forward one step
            else if (current.Type.Equals(Token.TokenType.Whitespace))
            {
                return true;
            }
            else if (current.Type.Equals(Token.TokenType.Quote))
            {
                if (inRecursion)
                    return false;
                else
                    throw new SyntaxError(current.LineNum, "Quote not allowed here");
            }
            else
            {
                Log.Debug("Could not parse " + current.Type + " instruction at index " + _currentToken + ".");
                return false;
            }
        }

        private Instruction HandleShort(Token current)
        {
            Token next = null;

            Expect(Token.TokenType.Dot, " at end of " + current + " instruction", ref next);
            return new Instruction(current.Type);
        }

        private Instruction HandleReg(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.Whitespace, "after INSTR", ref next);

            // Followed by a number
            Expect(Token.TokenType.Number, "after whitespace token", ref next);
            var num = next.Num;

            Expect(Token.TokenType.Dot, " at end of " + current + " instruction", ref next);
            return new Instruction(current.Type, num);
        }

        private Instruction HandleColor(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.Whitespace, "after color token", ref next);

            // Followed by a Hex color code
            Expect(Token.TokenType.Hex, "after whitespace", ref next);
            var hex = next.Hex;

            Expect(Token.TokenType.Dot, " at end of " + current + " instruction", ref next);
            return new Instruction(Token.TokenType.Color, hex);
        }

        private Instruction HandleRep()
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.Whitespace, "after Rep token", ref next);

            // Followed by a number, which is saved
            Expect(Token.TokenType.Number, "after Rep + whitespace", ref next);
            var num = next.Num;

            // Followed by whitespace again
            Expect(Token.TokenType.Whitespace, "after Rep number", ref next);

            // Get the token determining Rep Type
            var determinator = PeekToken();

            while (determinator.Type.Equals(Token.TokenType.Whitespace))
            {
                Log.Debug("Ignored whitespace token " + determinator);
                determinator = PeekToken();
                if (determinator.Type.Equals(Token.TokenType.Whitespace))
                    determinator = NextToken();
            }

            Log.Debug("Determinator: " + determinator);

            // Look at the determinator token to see whether one or multiple instructions
            // should be repeated
            if (determinator.Type.Equals(Token.TokenType.Quote))
            {
                // Eat up the determinator quote
                NextToken();
                
                // Parse statements recursively
                // This will run until it encounters a lone quote token (not part of a Rep start),
                // once it does the Statement(), command will fail, thus kicking back here
                var recursiveList = new List<Instruction>();
                Log.Debug("----> Begin Rep recursion ...");
                StatementList(recursiveList, true);
                Log.Debug("----> Ended Rep recursion, parsed " + recursiveList.Count + " statements");

                // By now, other code will have swallowed our expected quote, so we have to peek back
                next = tokens.ElementAt(_currentToken - 1);

                if (!next.Type.Equals(Token.TokenType.Quote))
                {
                    Log.Debug("Quote expected after Rep statements");
                    throw new SyntaxError(tokens.ElementAt(_currentToken - 2).LineNum, "Quote expected after Rep statements, got " + next.Type);
                }    

                return new Instruction(Token.TokenType.Rep, num, recursiveList);
            }

            // To repeat a single instruction, just make a single statement parse
            var rep = new List<Instruction>();
            Statement(rep, true);
            return new Instruction(Token.TokenType.Rep, num, rep);
        }

        /// <summary>
        /// Expect a token to be next in line, throwing a SyntaxError otherwise
        /// </summary>
        /// <param name="type">Token Type to expect</param>
        /// <param name="message">Error information, in the format of "Expected Y, {message}, got Z"</param>
        /// <param name="next">A reference to the Token object used for forward-lookup</param>
        private void Expect(Token.TokenType type, string message, ref Token next)
        {
            next = NextToken();
            var initial = next;

            // If we're not specifically expecting whitespace, we can safely ignore any such
            if (!type.Equals(Token.TokenType.Whitespace))
            {
                while (next.Type.Equals(Token.TokenType.Whitespace))
                {
                    Log.Debug("---> Not expecting whitespace, expecting " + type + ", ignored whitespace token " + next + " at line " + next.LineNum);

                    if (HasMoreTokens())
                        next = NextToken();
                    else
                        throw new SyntaxError(initial.LineNum, "No valid continuation of " + initial.Type + " token");
                }
            }

            if (!next.Type.Equals(type))
                throw new SyntaxError(next.LineNum, "Expected " + type + " " + message.Trim() + ", got " + next.Type + " at line " + next.LineNum);
        }

        private bool HasMoreTokens()
        {
            return _currentToken < tokens.Count;
        }

        private Token PeekToken()
        {
            if (!HasMoreTokens())
                throw new SyntaxError(tokens.Last().LineNum, "Tried to peek token when no more tokens exist");

            return tokens.ElementAt(_currentToken);
        }

        private Token NextToken()
        {
            var t = PeekToken();
            _currentToken++;
            return t;
        }

    }
}
