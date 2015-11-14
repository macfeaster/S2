using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
	class Lexer
	{
		public List<string> getInput()
		{
			List<string> input = new List<string>();
			string line;

			while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
			{
				int commentIndex = line.IndexOf("%");

				if (commentIndex > 0)
					line = line.Substring(0, commentIndex);
				else if (commentIndex == 0)
					line = "";

				input.Add(line);
			}

			return input;
		}
	}
}
