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

            string program3 = "% Nästlad loop 1" + Environment.NewLine +
                                "REP 2 \"UP.FORW 10.DOWN.REP 3 \"LEFT 120. FORW 1.\"\"" + Environment.NewLine +
                                "% Nästlad loop 2" + Environment.NewLine +
                                "REP 3 \"REP 2 \"RIGHT 2. FORW 1.\"" + Environment.NewLine +
                                "       COLOR #FF0000. FORW 10. COLOR #0000FF.\"" + Environment.NewLine +
                                "% COLOR #000000. % Bortkommenterat färgbyte" + Environment.NewLine +
                                "BACK 10." + Environment.NewLine +
                                "% Upper/lower case ignoreras" + Environment.NewLine +
                                "% Detta gäller även hex-tecknen A-F i färgerna i utdata," + Environment.NewLine +
                                "% det spelar ingen roll om du använder stora eller små" + Environment.NewLine +
                                "% bokstäver eller en blandning." + Environment.NewLine +
                                "color #AbcdEF. left 70. foRW 10." + Environment.NewLine;

            try
            {
                Lexer l = new Lexer();

                #if DEBUG
                    string preprocessed = l.FilterInput(program3);
                #else
                    string input = l.GetInput();
                    string preprocessed = l.FilterInput(input);
                #endif

                List<Token> parsed = l.Parse(preprocessed);

                Parser p = new Parser(parsed);
                List<Instruction> tree = p.GetTree();

                Console.WriteLine(string.Join(Environment.NewLine, tree));

                #if DEBUG
                    Console.WriteLine(string.Join(Environment.NewLine, tree));
                #endif
            }
            catch (SyntaxError e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
	}
}
