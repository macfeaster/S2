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

            string program2 = "% Space runt punkt valfritt." + Environment.NewLine +
                                "DOWN  . UP.DOWN.  DOWN." + Environment.NewLine +
                                "% Rader kan vara tomma" + Environment.NewLine +
                                "" + Environment.NewLine +
                                "% radbrytning/space/tabb för" + Environment.NewLine +
                                "% att göra koden mer läslig." + Environment.NewLine +
                                "REP 3 \"COLOR ##FF0000." + Environment.NewLine +
                                "       FORW 1. LEFT 10." + Environment.NewLine +
                                "       COLOR #000000." + Environment.NewLine +
                                "       FORW 2. LEFT 20.\"" + Environment.NewLine +
                                "% Eller oläslig" + Environment.NewLine +
                                "           COLOR" + Environment.NewLine +
                                "% färgval på gång" + Environment.NewLine +
                                "  #111111." + Environment.NewLine +
                                "REP 1 BACK 1." + Environment.NewLine;

            Console.WriteLine("FORW 1. LEFT 90.\n".IndexOf('\n'));

            /* try
            {*/
                Lexer l = new Lexer();
                string preprocessed = l.FilterInput(program2);
                List<Token> parsed = l.Parse(preprocessed);

                Parser p = new Parser(parsed);
                List<Instruction> tree = p.GetTree();

            Console.WriteLine(string.Join(", ", tree));
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
