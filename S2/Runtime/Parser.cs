// SyntaxError.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

using System.Collections.Generic;
using System.Linq;

namespace S2
{
    /// <summary>
    /// Parses a List of tokens into a List of instructions, or throws SyntaxError upon parsing failure.
    /// </summary>
    internal class Parser
    {
        // Data structures for token list, output instruction list and token pointer
        private readonly List<Token> _tokens;
        List<Instruction> _instructions;
        private int _currentToken;

        /// <summary>
        /// Entry point, initializes parsing. Throws SyntaxError if any tokens remain after parsing completes.
        /// </summary>
        public Parser(List<Token> tokens)
        {
            // Set the tokens field
            _tokens = tokens;
            _instructions = new List<Instruction>();

            // Handle empty token list
            if (!HasMoreTokens())
                throw new SyntaxError(1, "No _tokens to parse");

            // Parse tokens using recursion, throwing SyntaxError if quote
            // is encountered, since we are not currently in a recursion context
            StatementList(_instructions, false);

            // If tokens remain after parsing completes, throw a SyntaxError
            if (HasMoreTokens())
                throw new SyntaxError(tokens.Last().LineNum, "Tokens left after parsing completed, unexpected token " + PeekToken());
        }

        /// <summary>
        /// Get the Instruction list produced by the Parser
        /// </summary>
        public List<Instruction> GetTree()
        {
            return _instructions;
        }

        /// <summary>
        /// Parse statements from _tokens recursively.
        /// </summary>
        private void StatementList(List<Instruction> output, bool inRecursion)
        {
            // Save whether a statement was successfully parsed into an Instruction
            var statementParsed = Statement(output, inRecursion);

            // If there are more tokens to parse, and the last parse did not fail,
            // continue parsing of statements
            if (HasMoreTokens() && statementParsed)
            {
                StatementList(output, inRecursion);
            }
            else
            {
                // We either ran out of tokens, or there is no point in parsing further
                // since the last parse failed
                Log.Debug("HasMoreTokens: " + HasMoreTokens());
                Log.Debug("_currentToken: " + _currentToken);
                Log.Debug("NumTokens: " + _tokens.Count);
                Log.Debug("StatementParsed: " + statementParsed);
                Log.Debug("Aborting parsing");
            }
        }

        /// <summary>
        /// Parse tokens into an Instruction, throwing SyntaxError upon failure.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="inRecursion"></param>
        /// 
        /// <returns>True if an Instruction is properly produced, false otherwise.</returns>
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
            // Handle short _instructions, i.e. Up. and Down.
            else if (shortInstr.Contains(current.Type))
            {
                Log.Debug("--> Called " + current.Type + " parser.");
                output.Add(HandleShort(current));
                Log.Debug("--> Parsed " + current.Type + " instruction.");
                return true;
            }
            // Handle regular _instructions, i.e. Left 2, Forw 10.
            else if (regInstr.Contains(current.Type))
            {
                Log.Debug("--> Called " + current.Type + " parser.");
                output.Add(HandleReg(current));
                Log.Debug("--> Parsed " + current.Type + " instruction.");
                return true;
            }
            // Handle Color _instructions
            else if (current.Type.Equals(Token.TokenType.Color))
            {
                Log.Debug("--> Called " + current.Type + " parser.");
                output.Add(HandleColor(current));
                Log.Debug("--> Parsed " + current.Type + " instruction.");
                return true;
            }
            // Handle Rep _instructions
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
            // Quote tokens are only allowed if we are in a REP recursion, where they
            // end the current recursion by returning false (thus closing the current StatementList parse)
            else if (current.Type.Equals(Token.TokenType.Quote))
            {
                if (inRecursion)
                    return false;
                else
                    throw new SyntaxError(current.LineNum, "Quote not allowed here");
            }
            // This should never be reached
            else
            {
                Log.Debug("Could not parse " + current.Type + " instruction at index " + _currentToken + ".");
                return false;
            }
        }

        /// <summary>
        /// Parse a short instruction, i.e. UP or DOWN followed by a dot.
        /// </summary>
        private Instruction HandleShort(Token current)
        {
            Token next = null;

            // Has to end with a dot
            Expect(Token.TokenType.Dot, " at end of " + current + " instruction", ref next);
            return new Instruction(current.Type);
        }

        /// <summary>
        /// Parse a regular instruction, i.e. LEFT, RIGHT, BACK, FORW followed by a number and a dot.
        /// The instruction and the number must be separated by whitespace.
        /// </summary>
        private Instruction HandleReg(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.Whitespace, "after INSTR", ref next);

            // Followed by a number
            Expect(Token.TokenType.Number, "after whitespace token", ref next);
            var num = next.Num;

            // Followed by a dot
            Expect(Token.TokenType.Dot, " at end of " + current + " instruction", ref next);
            return new Instruction(current.Type, num);
        }

        /// <summary>
        /// Parse a color instruction, i.e. COLOR followed by a hex color code.
        /// The Color instruction and the hex code must be separated by whitespace.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private Instruction HandleColor(Token current)
        {
            Token next = null;

            // Has to contain a whitespace
            Expect(Token.TokenType.Whitespace, "after color token", ref next);

            // Followed by a Hex color code
            Expect(Token.TokenType.Hex, "after whitespace", ref next);
            var hex = next.Hex;

            // Followed by a dot
            Expect(Token.TokenType.Dot, " at end of " + current + " instruction", ref next);
            return new Instruction(Token.TokenType.Color, hex);
        }

        /// <summary>
        /// Parse a rep instruction, which is either REP followed by a number and a single
        /// instruction, or REP followed by a number and multiple instructions, surrounded by quotes.
        /// </summary>
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

            // As long as the determinator is whitespace, peek forward and optionally
            // ignore more whitspace, until an actual determinator is encountered
            while (determinator.Type.Equals(Token.TokenType.Whitespace))
            {
                Log.Debug("Ignored whitespace token " + determinator);
                determinator = PeekToken();

                if (determinator.Type.Equals(Token.TokenType.Whitespace))
                    determinator = NextToken();
            }

            Log.Debug("Determinator: " + determinator);

            // Look at the determinator token to see whether one or multiple _instructions
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
                next = _tokens.ElementAt(_currentToken - 1);

                if (!next.Type.Equals(Token.TokenType.Quote))
                {
                    Log.Debug("Quote expected after Rep statements");
                    throw new SyntaxError(_tokens.ElementAt(_currentToken - 2).LineNum, "Quote expected after Rep statements, got " + next.Type);
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
        /// 
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

            // Look at the next token to see whether it matches the expectation provided
            if (!next.Type.Equals(type))
                throw new SyntaxError(next.LineNum, "Expected " + type + " " + message.Trim() + ", got " + next.Type + " at line " + next.LineNum);
        }

        //
        // HELPER FUNCTIONS
        // 

        /// <summary>
        /// Checks whether any more tokens are available.
        /// </summary>
        private bool HasMoreTokens()
        {
            return _currentToken < _tokens.Count;
        }

        /// <summary>
        /// Peek forward, grabbing the upcoming token without moving the token pointer.
        /// Throws SyntaxError if called when no more tokens exist.
        /// </summary>
        private Token PeekToken()
        {
            if (!HasMoreTokens())
                throw new SyntaxError(_tokens.Last().LineNum, "Tried to peek token when no more _tokens exist");

            return _tokens.ElementAt(_currentToken);
        }

        /// <summary>
        /// Grab the upcoming token, and move the token pointer one step forward.
        /// </summary>
        private Token NextToken()
        {
            var t = PeekToken();
            _currentToken++;
            return t;
        }

    }
}
