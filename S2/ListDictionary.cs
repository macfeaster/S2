using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
    class ListDictionary : SortedDictionary<int, List<Token>>
    {
        private int currentLine = 0;
        private int currentToken = 0;

        public void Add(int lineNumber, Token token)
        {
            List<Token> existing;

            if (this.TryGetValue(lineNumber, out existing))
            {
                existing = new List<Token>();
                this[lineNumber] = existing;
            }

            existing.Add(token);
        }

        /// <summary>
        /// Get the next token in line from the Lexer parsing.
        /// When getting the next token, first try to check
        /// whether current line has more tokens. If it does not
        /// proceed by resetting currentToken and trying the next line.
        /// If both fail, throw SyntaxError - there are no more available tokens.
        /// </summary>
        /// <returns>Next Token object, if available</returns>
        public Token NextToken()
        {
            var lineTokens = new List<Token>();

            // Check if current line exists
            if (this.TryGetValue(currentLine, out lineTokens))
            {
                // Check if a new token exists on this line
                if (lineTokens.Count > currentToken)
                {
                    currentToken++;
                    return lineTokens.ElementAt(currentToken);
                }
                // We have to check next line
                else
                {
                    // Increase line counter, reset token counter
                    currentLine++;
                    currentToken = 0;

                    // If next line exists, get its first element and increase token counter
                    if (this.TryGetValue(currentLine, out lineTokens))
                    {
                        currentToken++;
                        return lineTokens.First();
                    }
                    else
                        throw new SyntaxError(currentLine - 1);
                }
            }
            else
                throw new SyntaxError(currentLine);
        }
    }
}
