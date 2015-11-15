using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static S2.Token;

namespace S2
{
    class Lexer
	{

		public string getInput()
		{
			StringBuilder input = new StringBuilder();
			string line;

			while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
			{
				int commentIndex = line.IndexOf("%");

				if (commentIndex > 0)
					line = line.Substring(0, commentIndex);
				else if (commentIndex == 0)
					line = "";

				input.Append(line);
			}

			return input.ToString();
		}

		public List<Token> parse(string input)
		{
            // Set up regex tools
			string pattern = @"(DOWN|UP|FORW|BACK|LEFT|RIGHT|COLOR|REP|[0-9]+|[.]|""|[#][0-9A-F]{6}|\n)";
			Regex r = new Regex(pattern);
			MatchCollection matches = r.Matches(input);

            // Parsed tokens are placed in a list, lineCount keeps track of which line errors occur on
    		List<Token> tokens = new List<Token>();
			int lineCount = 0;

			foreach (Match m in matches)
			{
                switch (m.Value)
				{
					case "DOWN":
						tokens.Add(new Token(TokenType.DOWN));
						break;
					case "UP":
						tokens.Add(new Token(TokenType.UP));
                        break;
                    case "FORW":
                        tokens.Add(new Token(TokenType.FORW));
                        break;
                    case "BACK":
                        tokens.Add(new Token(TokenType.BACK));
                        break;
                    case "LEFT":
                        tokens.Add(new Token(TokenType.LEFT));
                        break;
                    case "RIGHT":
                        tokens.Add(new Token(TokenType.RIGHT));
                        break;
                    case "COLOR":
                        tokens.Add(new Token(TokenType.COLOR));
                        break;
                    case "REP":
                        tokens.Add(new Token(TokenType.REP));
                        break;
                    case ".":
                        tokens.Add(new Token(TokenType.DOT));
                        break;
                    case @"""":
                        tokens.Add(new Token(TokenType.QUOTE));
                        break;
                    case @"\n":
                        lineCount++;
                        break;

                    // Token has to be either a number or a hex value
                    default:
                        Console.WriteLine($"Matched value: {m.Value}");
                        throw new SyntaxError(lineCount);
				}
			}
		}
	}
}
