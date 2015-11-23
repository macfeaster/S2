// Program.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

using System;
using System.Collections.Generic;

namespace S2
{
    static class Program
    {
        public static void Main(string[] args)
        {
            // Test code for debug runs, ignored in production scenarios
            #region Test Code
            var program = new List<string>()
            {
                "% Det här är en kommentar",
                "% Nu ritar vi en kvadrat",
                "Down.",
                "Forw 1. Left 90.",
                "Forw 1. Left 90.",
                "Forw 1. Left 90.",
                "Forw 1. Left 90."
            };

            var program2 = new List<string>()
            {
                "% Space runt punkt valfritt.",
                "Down  . Up.Down.  Down.",
                "% Rader kan vara tomma",
                "",
                "% radbrytning/space/tabb för",
                "% att göra koden mer läslig.",
                "Rep 3 \"Color #FF0000.",
                "       Forw 1. Left 10.",
                "       Color #000000.",
                "       Forw 2. Left 20.\"",
                "% Eller oläslig",
                "           Color",
                "% färgval på gång",
                "  #111111.",
                "Rep 1 Back 1."
            };

            var program3 = new List<string>()
            {
                "% Syntaxfel: felaktig färgsyntax",
                "Color 05AB34.",
                "Forw 1."
            };

            var program4 = new List<string>()
            {
                "% Oavslutad loop",
                "Rep 5 \"Down. Forw 1. Left 10."
            };

            var program5 = new List<string>()
            {
                "% Syntaxfel: ej heltal",
                "Forw 2,3."
            };

            var program6 = new List<string>()
            {
                "%&(CDH*(",
                "Forw",
                "#123456.",
                "& C(*N & (*#NRC"
            };

            var program7 = new List<string>()
            {
                "% Måste vara whitespace mellan",
                "% kommando och parameter",
                "Down.Color#000000."
            };

            var program8 = new List<string>()
            {
                "% Syntaxfel: saknas punkt.",
                "Down ",
                "% Om filen tar slut mitt i ett kommando",
                "% så anses felet ligga på sista raden ",
                "   % i filen där det förekom någon kod"
            };

            var program9 = new List<string>()
            {
                "% Måste vara space mellan argument",
                "Rep  5\"Forw 1.\"",
                "% Detta inte OK heller",
                "Rep   5FORW 1."
            };

            var program10 = new List<string>()
            {
                "% Nästlad loop 1",
                "Rep 2 \"Up.Forw 10.Down.Rep 3 \"Left 120. Forw 1.\"\"",
                "% Nästlad loop 2",
                "Rep 3 \"Rep 2 \"Right 2. Forw 1.\"",
                "       Color #FF0000. Forw 10. Color #0000FF.\"",
                "% Color #000000. % Bortkommenterat färgbyte",
                "Back -10.",
                "% Upper/lower case ignoreras",
                "% Detta gäller även Hex-tecknen A-F i färgerna i utdata,",
                "% det spelar ingen roll om du använder stora eller små",
                "% bokstäver eller en blandning.",
                "color #AbcdEF. left 70. foRW 10."
            };

            var program11 = new List<string>()
            {
                "% Ta 8 steg framåt",
                "Rep 2 Rep 4 Forw 1.",
                "Rep% Repetition på gång",
                "2% Två gånger",
                "\" % Snart kommer kommandon",
                "Down% Kommentera mera",
                ".% Avsluta down-kommando",
                "Forw 1",
                "Left 1. % Oj, glömde punkt efter Forw-kommando",
                "\""
            };

            var program23 = new List<string>()
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
                "Up.",
                "FO",
                "RW 100."
            };

            var program28 = new List<string>()
            {
                "% Oavslutad loop",
                "Rep 5 \"Down. Forw 1. Left 10.",
                "Left 10."
            };

            var program31 = new List<string>()
            {
                "#ABCEEF 03 Rep 4 #EFEFEF 3."
            };

            var program34 = new List<string>
            {
                "Up. Down.",
                "Down.."
            };

            #endregion

            // Try to run the program, catching any SyntaxErrors happening along the
            // way, printing them out to the command line
            try
            {
                var l = new Lexer();

                // Debug a selected program, or get the input from stdin in production
                // Pre-process the input by removing code comments
                #if DEBUG
                    var preprocessed = l.FilterInput(program);
                #else
                    List<string> input = l.GetInput();
                    List<string> preprocessed = l.FilterInput(input);
                #endif

                // Lex the pre-processed data into tokens
                var parsed = l.Parse(preprocessed);

                #if DEBUG
                    Console.WriteLine(string.Join(Environment.NewLine, parsed));
                #endif

                // Parse tokens into an instruction tree, where Instructions
                // are supplied in a list, and sub-lists of instructions branch
                // out from the main line
                var p = new Parser(parsed);
                var tree = p.GetTree();

                #if DEBUG
                    Console.WriteLine(string.Join(Environment.NewLine, tree));
                #endif

                // Run the instruction tree
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
