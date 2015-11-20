using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
	class Program
	{
		static void Main(string[] args)
		{
            string program = @"% Det här är en kommentar" + Environment.NewLine +
                                "% Nu ritar vi en kvadrat" + Environment.NewLine +
                                "DOWN." + Environment.NewLine +
                                "FORW 1. LEFT 90." + Environment.NewLine +
                                "FORW 1. LEFT 90." + Environment.NewLine +
                                "FORW 1. LEFT 90." + Environment.NewLine;

            Console.WriteLine("FORW 1. LEFT 90.\n".IndexOf('\n'));

            /* try
            {*/
                Lexer l = new Lexer();
                string preprocessed = l.FilterInput(program);
                List<Token> parsed = l.Parse(preprocessed);

                Parser p = new Parser(parsed);
                List<Instruction> tree = p.GetTree();
            /* }
            catch (SyntaxError e)
            {
                Console.WriteLine(e.Message);
            } */

            Console.WriteLine("DONE PARSING");

            Console.ReadLine();
        }
	}
}
