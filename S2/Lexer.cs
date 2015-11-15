using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace S2
{
	class Lexer
	{
		List<Token> tokens = new List<Token>();

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
			string pattern = "(DOWN|UP|FORW|BACK|LEFT|RIGHT|COLOR|REP|[0-9]+|[.]|\"|[#][0-9A-F]{6}|\n)";
			Regex r = new Regex(pattern);

			MatchCollection matches = r.Matches(input);

			foreach (Match m in matches)
			{

			}
		}
	}
}
