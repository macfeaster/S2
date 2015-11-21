using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace S2
{
    class Lexer
	{
        List<Token> tokens;

        /// <summary>
        /// Get input from stdin and build a string of it.
        /// </summary>
        /// <returns>Stdin input as string</returns>
		public string GetInput()
		{
			StringBuilder input = new StringBuilder();
			string line;

            // Read all input, even empty lines and whitespaces
			while ((line = Console.ReadLine()) != null)
			{
                
                input.Append(line);
                input.Append("\n");
			}

			return input.ToString();
		}

        public string FilterInput(string input)
        {
            string[] lines = input.Split('\n');
            StringBuilder output = new StringBuilder();

            foreach (string line in lines)
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
                output.Append(filteredLine.ToUpper());
            }

            return output.ToString();
        }

        /// <summary>
        /// Parse an input string into a ListDictionary of Tokens.
        /// </summary>
        /// 
        /// <param name="input">Code string to parse</param>
        /// <returns>ListDictionary of parsed tokens, with line number as key, and
        /// the Tokens that line contains in a List, as value.</returns>
		public List<Token> Parse(string input)
		{
            // Set up regex tools
			var pattern = @"(DOWN|UP|FORW|BACK|LEFT|RIGHT|COLOR|REP|[0-9]+|[.]|""|[#][0-9A-F]{6}|(\r|\n|\r\n)|\s+)";
			var r = new Regex(pattern);
			var matches = r.Matches(input);

            // Parsed tokens are placed in a list, lineCount keeps track of which line errors occur on
            tokens = new List<Token>();
			int lineNum = 1;
            int lexPos = 0;

			foreach (Match m in matches)
			{
                // Match regex string pattern matches with their token equivalents
                switch (m.Value)
				{
					case "DOWN":
                        Console.WriteLine("Recognized DOWN token on line " + lineNum);
						tokens.Add(new Token(lineNum, Token.TokenType.DOWN));
						break;
					case "UP":
                        Console.WriteLine("Recognized UP token on line " + lineNum);
                        tokens.Add(new Token(lineNum, Token.TokenType.UP));
                        break;
                    case "FORW":
                        tokens.Add(new Token(lineNum, Token.TokenType.FORW));
                        Console.WriteLine("Recognized FORW token on line " + lineNum);
                        break;
                    case "BACK":
                        tokens.Add(new Token(lineNum, Token.TokenType.BACK));
                        Console.WriteLine("Recognized BACK token on line " + lineNum);
                        break;
                    case "LEFT":
                        tokens.Add(new Token(lineNum, Token.TokenType.LEFT));
                        Console.WriteLine("Recognized LEFT token on line " + lineNum);
                        break;
                    case "RIGHT":
                        tokens.Add(new Token(lineNum, Token.TokenType.RIGHT));
                        Console.WriteLine("Recognized RIGHT token on line " + lineNum);
                        break;
                    case "COLOR":
                        tokens.Add(new Token(lineNum, Token.TokenType.COLOR));
                        Console.WriteLine("Recognized COLOR token on line " + lineNum);
                        break;
                    case "REP":
                        tokens.Add(new Token(lineNum, Token.TokenType.REP));
                        Console.WriteLine("Recognized REP token on line " + lineNum);
                        break;
                    case ".":
                        tokens.Add(new Token(lineNum, Token.TokenType.DOT));
                        Console.WriteLine("Recognized DOT token on line " + lineNum);
                        break;
                    case @"""":
                        tokens.Add(new Token(lineNum, Token.TokenType.QUOTE));
                        Console.WriteLine("Recognized QUOTE token on line " + lineNum);
                        break;
                    case @"\n":
                        Console.WriteLine("Recognized NEWLINE token on line " + (lineNum + 1));
                        lineNum++;
                        break;

                    // Token has to be either a number or a hex value
                    default:
                        // Capture numeric value, if possible
                        // If numeric, add a NUMBER token
                        int val;
                        if (int.TryParse(m.Value, out val))
                        {
                            Console.WriteLine("Recognized NUMBER token on line " + lineNum);
                            tokens.Add(new Token(lineNum, Token.TokenType.NUMBER, val));
                        }
                        // A seven character string, starting with #, is a hex color code
                        // match of our hex regex pattern
                        else if (m.Value.StartsWith("#") && m.Value.Length == 7)
                        {
                            Console.WriteLine("Recognized HEX token on line " + lineNum);
                            tokens.Add(new Token(lineNum, Token.TokenType.HEX, m.Value));
                        }
                        // A matched value which is null or whitespace and is not null
                        // is whitespace
                        else if (string.IsNullOrWhiteSpace(m.Value) && m.Value != null)
                        {
                            if (m.Value.IndexOf('\n') >= 0 || m.Value.IndexOf('\r') >= 0)
                            {
                                Console.WriteLine("Recognized NEWLINE token on line " + (lineNum + 1));
                                lineNum++;
                            }
                            else
                            {
                                Console.WriteLine("Recognized WHITESPACE token on line " + lineNum);
                                tokens.Add(new Token(lineNum, Token.TokenType.WHITESPACE));
                            }
                        }
                        // The matcher has encountered unknown data
                        else
                            throw new SyntaxError(lineNum, "Encountered unknown data: " + m.Value);
                        break;
				}

                lexPos = m.Index + m.Value.Length;
			}

            // If there is still unparsed data, there is something illegal that cannot be lexed
            if (lexPos != input.Length)
                throw new SyntaxError(lineNum,
                    "Remaining illegal tokens: " + input.Substring(lexPos));

            return tokens;
		}
	}
}
