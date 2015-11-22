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
            List<string> program = new List<string>()
            {
                "% Det här är en kommentar",
                "% Nu ritar vi en kvadrat",
                "DOWN.",
                "FORW 1. LEFT 90.",
                "FORW 1. LEFT 90.",
                "FORW 1. LEFT 90.",
                "FORW 1. LEFT 90."
            };

            List<string> program2 = new List<string>()
            {
                "% Space runt punkt valfritt.",
                "DOWN  . UP.DOWN.  DOWN.",
                "% Rader kan vara tomma",
                "",
                "% radbrytning/space/tabb för",
                "% att göra koden mer läslig.",
                "REP 3 \"COLOR #FF0000.",
                "       FORW 1. LEFT 10.",
                "       COLOR #000000.",
                "       FORW 2. LEFT 20.\"",
                "% Eller oläslig",
                "           COLOR",
                "% färgval på gång",
                "  #111111.",
                "REP 1 BACK 1."
            };

            List<string> program3 = new List<string>()
            {
                "% Syntaxfel: felaktig färgsyntax",
                "COLOR 05AB34.",
                "FORW 1."
            };

            List<string> program4 = new List<string>()
            {
                "% Oavslutad loop",
                "REP 5 \"DOWN. FORW 1. LEFT 10."
            };

            List<string> program5 = new List<string>()
            {
                "% Syntaxfel: ej heltal",
                "FORW 2,3."
            };

            List<string> program6 = new List<string>()
            {
                "%&(CDH*(",
                "FORW",
                "#123456.",
                "& C(*N & (*#NRC"
            };

            List<string> program7 = new List<string>()
            {
                "% Måste vara whitespace mellan",
                "% kommando och parameter",
                "DOWN.COLOR#000000."
            };

            List<string> program8 = new List<string>()
            {
                "% Syntaxfel: saknas punkt.",
                "DOWN ",
                "% Om filen tar slut mitt i ett kommando",
                "% så anses felet ligga på sista raden ",
                "   % i filen där det förekom någon kod"
            };

            List<string> program9 = new List<string>()
            {
                "% Måste vara space mellan argument",
                "REP  5\"FORW 1.\"",
                "% Detta inte OK heller",
                "REP   5FORW 1."
            };

            List<string> program10 = new List<string>()
            {
                "% Nästlad loop 1",
                "REP 2 \"UP.FORW 10.DOWN.REP 3 \"LEFT 120. FORW 1.\"\"",
                "% Nästlad loop 2",
                "REP 3 \"REP 2 \"RIGHT 2. FORW 1.\"",
                "       COLOR #FF0000. FORW 10. COLOR #0000FF.\"",
                "% COLOR #000000. % Bortkommenterat färgbyte",
                "BACK -10.",
                "% Upper/lower case ignoreras",
                "% Detta gäller även hex-tecknen A-F i färgerna i utdata,",
                "% det spelar ingen roll om du använder stora eller små",
                "% bokstäver eller en blandning.",
                "color #AbcdEF. left 70. foRW 10."
            };

            List<string> program11 = new List<string>()
            {
                "% Ta 8 steg framåt",
                "REP 2 REP 4 FORW 1.",
                "REP% Repetition på gång",
                "2% Två gånger",
                "\" % Snart kommer kommandon",
                "DOWN% Kommentera mera",
                ".% Avsluta down-kommando",
                "FORW 1",
                "LEFT 1. % Oj, glömde punkt efter FORW-kommando",
                "\""
            };

            List<string> program23 = new List<string>()
            {
                "#*Ä¤*ÖÄ*%&Ä#&*JYco1hjmtHCaq03bzIm4" +
                "8UzxhBeJSd" +
                "9u8KdzHJUR" +
                "RitD8GbTWE" +
                "j2tkkrRxA3" +
                "f1gtbFgPJ6"
            };

            var program26 = new List<string>()
            {
                "UP.",
                "FO",
                "RW 100."
            };

            try
            {
                var l = new Lexer();

                #if DEBUG
                    var preprocessed = l.FilterInput(program26);
                #else
                    List<string> input = l.GetInput();
                    List<string> preprocessed = l.FilterInput(input);
                #endif

                var parsed = l.Parse(preprocessed);

                #if DEBUG
                    Console.WriteLine(string.Join(Environment.NewLine, parsed));
                #endif

                var p = new Parser(parsed);
                var tree = p.GetTree();

                #if DEBUG
                    Console.WriteLine(string.Join(Environment.NewLine, tree));
                #endif

                var r = new Runner(tree);

            }
            catch (SyntaxError e)
            {
                Console.WriteLine(e.Message);
            }

            #if DEBUG
                Console.ReadLine();
            #endif
        }
	}
}
