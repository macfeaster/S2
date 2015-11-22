// Lexer.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace S2
{
    /// <summary>
    /// Converts a List of string elements to Token objects.
    /// </summary>
    internal class Lexer
	{
        private List<Token> _tokens;

        /// <summary>
        /// Get input from stdin and build a string of it.
        /// </summary>
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

        /// <summary>
        /// Filter every line of an input List, by removing code comments and upper-casing it.
        /// </summary>
        public List<string> FilterInput(List<string> input)
        {
            var output = new List<string>();

            foreach (var line in input)
            {
                // Check whether line has a comment
                string filteredLine;
                var commentIndex = line.IndexOf("%");

                // Chop off lines with trailing comments
                if (commentIndex > 0)
                    filteredLine = line.Substring(0, commentIndex);
                // Replace comment lines with empty lines
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
        /// Parse an input string into a List of Tokens.
        /// </summary>
		public List<Token> Parse(List<string> input)
		{
            // Set up regex tools
			var pattern = @"(DOWN|UP|FORW|BACK|LEFT|RIGHT|COLOR|REP|[0-9]+|[#][0-9A-F]{6}|[.]|""|\s+)";
			var r = new Regex(pattern);

            // Parsed _tokens are placed in a list, lineCount keeps track of which line errors occur on
            _tokens = new List<Token>();
            var lineNum = 0;

            foreach (var line in input)
            { 
			    var matches = r.Matches(line);
                lineNum++;
                var lexPos = 0;

			    foreach (Match m in matches)
			    {
                    // Match regex string pattern matches with their token equivalents
                    switch (m.Value)
				    {
					    case "DOWN":
                            Log.Debug("Recognized Down token on line " + lineNum);
						    _tokens.Add(new Token(lineNum, Token.TokenType.Down));
						    break;
					    case "UP":
                            Log.Debug("Recognized Up token on line " + lineNum);
                            _tokens.Add(new Token(lineNum, Token.TokenType.Up));
                            break;
                        case "FORW":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Forw));
                            Log.Debug("Recognized Forw token on line " + lineNum);
                            break;
                        case "BACK":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Back));
                            Log.Debug("Recognized Back token on line " + lineNum);
                            break;
                        case "LEFT":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Left));
                            Log.Debug("Recognized Left token on line " + lineNum);
                            break;
                        case "RIGHT":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Right));
                            Log.Debug("Recognized Right token on line " + lineNum);
                            break;
                        case "COLOR":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Color));
                            Log.Debug("Recognized Color token on line " + lineNum);
                            break;
                        case "REP":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Rep));
                            Log.Debug("Recognized Rep token on line " + lineNum);
                            break;
                        case ".":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Dot));
                            Log.Debug("Recognized Dot token on line " + lineNum);
                            break;
                        case @"""":
                            _tokens.Add(new Token(lineNum, Token.TokenType.Quote));
                            Log.Debug("Recognized Quote token on line " + lineNum);
                            break;

                        // Token has to be either a number or a Hex value
                        default:
                            // Capture numeric value, if possible
                            // If numeric, add a Number token
                            int val;
                            if (int.TryParse(m.Value, out val))
                            {
                                Log.Debug("Recognized Number token on line " + lineNum);
                                _tokens.Add(new Token(lineNum, Token.TokenType.Number, val));
                            }
                            // A seven character string, starting with #, is a Hex color code
                            // match of our Hex regex pattern
                            else if (m.Value.StartsWith(@"#") && m.Value.Length == 7)
                            {
                                Log.Debug("Recognized Hex token on line " + lineNum);
                                _tokens.Add(new Token(lineNum, Token.TokenType.Hex, m.Value));
                            }
                            // A matched value which is null or whitespace and is not null
                            // is whitespace
                            else if (string.IsNullOrWhiteSpace(m.Value) && m.Value != null)
                            {
                                Log.Debug("Recognized Whitespace token " + m.Value +
                                    string.Join(
                                        ", ",
                                        m.Value.ToCharArray().Select(i => (int)i)
                                    ) + " on line " + (lineNum));
                                _tokens.Add(new Token(lineNum, Token.TokenType.Whitespace));
                            }
                            // The matcher has encountered unknown data
                            else
                                _tokens.Add(new Token(lineNum, Token.TokenType.Invalid));
                            break;
				    }

                    // If the lexer jumped a symbol, we encountered something that could not be tokenized,
                    // and should be regarded as an Invalid token
                    if (lexPos + m.Value.Length != m.Index + m.Value.Length)
                    {
                        _tokens.Add(new Token(lineNum, Token.TokenType.Invalid));
                        Log.Debug("Jumped a symbol" + line.Substring(lexPos, m.Index - lexPos));
                        break;
                    }

                    lexPos = m.Index + m.Value.Length;
                }

                // A new line is viewed by the language spec as whitespace, so add one
                _tokens.Add(new Token(lineNum, Token.TokenType.Whitespace));

                // If the line contained something, but the matcher returned nothing, the whole line is invalid
                if (line.Length > 0 && matches.Count == 0)
                {
                    Log.Debug("Jumped a line: " + line);
                    _tokens.Add(new Token(lineNum, Token.TokenType.Invalid));
                    break;
                }

                // If there is still unparsed data, there is something illegal that cannot be lexed
                if (lexPos != line.Length)
                {
                    var last = _tokens.Last();
                    Log.Debug("Added invalid token on line " + last.LineNum);
                    _tokens.Add(new Token(last.LineNum, Token.TokenType.Invalid));
                    break;
                }
            }

            return _tokens;
		}
	}
}
