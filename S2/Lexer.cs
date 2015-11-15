﻿using System;
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

				input.Append(line.ToUpper());
			}

			return input.ToString();
		}

		public ListDictionary parse(string input)
		{
            // Set up regex tools
			var pattern = @"(DOWN|UP|FORW|BACK|LEFT|RIGHT|COLOR|REP|[0-9]+|[.]|""|[#][0-9A-F]{6}|\s|\n)";
			var r = new Regex(pattern);
			var matches = r.Matches(input);

            // Parsed tokens are placed in a list, lineCount keeps track of which line errors occur on
            var tokens = new ListDictionary();
			int lineNum = 0;
            int lexPos = 0;

			foreach (Match m in matches)
			{
                switch (m.Value)
				{
					case "DOWN":
						tokens.Add(lineNum, new Token(TokenType.DOWN));
						break;
					case "UP":
						tokens.Add(lineNum, new Token(TokenType.UP));
                        break;
                    case "FORW":
                        tokens.Add(lineNum, new Token(TokenType.FORW));
                        break;
                    case "BACK":
                        tokens.Add(lineNum, new Token(TokenType.BACK));
                        break;
                    case "LEFT":
                        tokens.Add(lineNum, new Token(TokenType.LEFT));
                        break;
                    case "RIGHT":
                        tokens.Add(lineNum, new Token(TokenType.RIGHT));
                        break;
                    case "COLOR":
                        tokens.Add(lineNum, new Token(TokenType.COLOR));
                        break;
                    case "REP":
                        tokens.Add(lineNum, new Token(TokenType.REP));
                        break;
                    case ".":
                        tokens.Add(lineNum, new Token(TokenType.DOT));
                        break;
                    case @"""":
                        tokens.Add(lineNum, new Token(TokenType.QUOTE));
                        break;
                    case @"\n":
                        lineNum++;
                        break;

                    // Token has to be either a number or a hex value
                    default:
                        // Capture numeric value, if possible
                        // If numeric, add a NUMBER token
                        int val;
                        if (int.TryParse(m.Value, out val))
                            tokens.Add(lineNum, new Token(TokenType.NUMBER, val));
                        else if (m.Value.StartsWith("#") && m.Value.Length == 7)
                            tokens.Add(lineNum, new Token(TokenType.HEX, m.Value));
                        else if (string.IsNullOrWhiteSpace(m.Value) && m.Value != null)
                            tokens.Add(lineNum, new Token(TokenType.WHITESPACE));
                        else
                            throw new SyntaxError(lineNum);
                        break;
				}
			}

            return tokens;
		}
	}
}
