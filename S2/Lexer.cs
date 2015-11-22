using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace S2
{
    internal class Lexer
	{
        private List<Token> _tokens;

        /// <summary>
        /// Get input from stdin and build a string of it.
        /// </summary>
        /// <returns>Stdin input as string</returns>
		public List<string> GetInput()
		{
            var input = new List<string>();
			string line;

            // Read all input, even empty lines and whitespaces
			while ((line = Console.ReadLine()) != null)
			{
                input.Add(line);
			}

			return input;
		}

        public List<string> FilterInput(List<string> input)
        {
            List<string> output = new List<string>();

            foreach (string line in input)
            {
                // Check whether line has a comment
                string filteredLine;
                int commentIndex = line.IndexOf("%");

                // Chop off lines with trailing comments
                // Replace comment lines with empty lines
                if (commentIndex > 0)
                    filteredLine = line.Substring(0, commentIndex);
                else if (commentIndex == 0)
                    filteredLine = "";
                else
                    filteredLine = line;

                // Uppercase and append with a newline character
                output.Add(filteredLine.ToUpper());
            }

            return output;
        }

        /// <summary>
        /// Parse an input string into a ListDictionary of Tokens.
        /// </summary>
        /// 
        /// <param name="input">Code string to parse</param>
        /// <returns>ListDictionary of parsed _tokens, with line number as key, and
        /// the Tokens that line contains in a List, as value.</returns>
		public List<Token> Parse(List<string> input)
		{
            // Set up regex tools
			var pattern = @"(DOWN|UP|FORW|BACK|LEFT|RIGHT|COLOR|REP|[0-9]+|[#][0-9A-F]{6}|[.]|""|\s+)";
			var r = new Regex(pattern);

            // Parsed _tokens are placed in a list, lineCount keeps track of which line errors occur on
            _tokens = new List<Token>();
            var lineNum = 0;

            foreach (string line in input)
            { 
			    var matches = r.Matches(line);
                lineNum++;

                int lexPos = 0;

			    foreach (Match m in matches)
			    {
                    // Match regex string pattern matches with their token equivalents
                    switch (m.Value)
				    {
					    case "DOWN":
                            Log.Debug("Recognized DOWN token on line " + lineNum);
						    _tokens.Add(new Token(lineNum, Token.TokenType.DOWN));
						    break;
					    case "UP":
                            Log.Debug("Recognized UP token on line " + lineNum);
                            _tokens.Add(new Token(lineNum, Token.TokenType.UP));
                            break;
                        case "FORW":
                            _tokens.Add(new Token(lineNum, Token.TokenType.FORW));
                            Log.Debug("Recognized FORW token on line " + lineNum);
                            break;
                        case "BACK":
                            _tokens.Add(new Token(lineNum, Token.TokenType.BACK));
                            Log.Debug("Recognized BACK token on line " + lineNum);
                            break;
                        case "LEFT":
                            _tokens.Add(new Token(lineNum, Token.TokenType.LEFT));
                            Log.Debug("Recognized LEFT token on line " + lineNum);
                            break;
                        case "RIGHT":
                            _tokens.Add(new Token(lineNum, Token.TokenType.RIGHT));
                            Log.Debug("Recognized RIGHT token on line " + lineNum);
                            break;
                        case "COLOR":
                            _tokens.Add(new Token(lineNum, Token.TokenType.COLOR));
                            Log.Debug("Recognized COLOR token on line " + lineNum);
                            break;
                        case "REP":
                            _tokens.Add(new Token(lineNum, Token.TokenType.REP));
                            Log.Debug("Recognized REP token on line " + lineNum);
                            break;
                        case ".":
                            _tokens.Add(new Token(lineNum, Token.TokenType.DOT));
                            Log.Debug("Recognized DOT token on line " + lineNum);
                            break;
                        case @"""":
                            _tokens.Add(new Token(lineNum, Token.TokenType.QUOTE));
                            Log.Debug("Recognized QUOTE token on line " + lineNum);
                            break;

                        // Token has to be either a number or a hex value
                        default:
                            // Capture numeric value, if possible
                            // If numeric, add a NUMBER token
                            int val;
                            if (int.TryParse(m.Value, out val))
                            {
                                Log.Debug("Recognized NUMBER token on line " + lineNum);
                                _tokens.Add(new Token(lineNum, Token.TokenType.NUMBER, val));
                            }
                            // A seven character string, starting with #, is a hex color code
                            // match of our hex regex pattern
                            else if (m.Value.StartsWith(@"#") && m.Value.Length == 7)
                            {
                                Log.Debug("Recognized HEX token on line " + lineNum);
                                _tokens.Add(new Token(lineNum, Token.TokenType.HEX, m.Value));
                            }
                            // A matched value which is null or whitespace and is not null
                            // is whitespace
                            else if (string.IsNullOrWhiteSpace(m.Value) && m.Value != null)
                            {
                                Log.Debug("Recognized WHITESPACE token " + m.Value +
                                    string.Join(
                                        ", ",
                                        m.Value.ToCharArray().Select(i => (int)i)
                                    ) + " on line " + (lineNum));
                                _tokens.Add(new Token(lineNum, Token.TokenType.WHITESPACE));
                            }
                            // The matcher has encountered unknown data
                            else
                                _tokens.Add(new Token(lineNum, Token.TokenType.INVALID));
                                // throw new SyntaxError(lineNum, "Encountered unknown data: " + m.Value);
                            break;
				    }

                    if (lexPos + m.Value.Length != m.Index + m.Value.Length)
                    {
                        _tokens.Add(new Token(lineNum, Token.TokenType.INVALID));
                        Log.Debug("Jumped a symbol" + line.Substring(lexPos, m.Index - lexPos));
                        // throw new SyntaxError(lineNum, "Jumped a symbol: " + line.Substring(lexPos, m.Index - lexPos));
                        break;
                    }

                    lexPos = m.Index + m.Value.Length;
                }

                _tokens.Add(new Token(lineNum, Token.TokenType.WHITESPACE));

                if (line.Length > 0 && matches.Count == 0)
                {
                    Log.Debug("Jumped a line: " + line);
                    _tokens.Add(new Token(lineNum, Token.TokenType.INVALID));
                    break;
                }

                // If there is still unparsed data, there is something illegal that cannot be lexed
                if (lexPos != line.Length)
                {
                    var last = _tokens.Last();
                    Log.Debug("Added invalid token on line " + last.lineNum);
                    _tokens.Add(new Token(last.lineNum, Token.TokenType.INVALID));
                    break;
                }
            }

            return _tokens;
		}
	}
}
