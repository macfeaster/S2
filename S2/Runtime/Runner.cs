// Runner.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

using System;
using System.Collections.Generic;

namespace S2
{
    /// <summary>
    /// Runs the Instructions supplied in a syntax tree produced by the parser.
    /// </summary>
    internal class Runner
    {
        private bool _penDown;
        private string _penColor = "#0000FF";
        private double _x;
        private double _y;
        private int _angle;

        public Runner(List<Instruction> tree)
        {
            Run(tree);
        }

        /// <summary>
        /// Run the instructions in the tree.
        /// </summary>
        private void Run(List<Instruction> tree)
        {
            // Traverse the instruction set
            foreach (var i in tree)
            {
                switch (i.Type)
                {
                    // The global pen state is changed by UP and DOWN commands
                    case Token.TokenType.Up:
                        Log.Debug("Pen is now Up");
                        _penDown = false;
                        break;
                    case Token.TokenType.Down:
                        Log.Debug("Pen is now Down");
                        _penDown = true;
                        break;

                    // FORW and BACK moves the pointer, drawing a line if the pen is down
                    case Token.TokenType.Forw:
                    case Token.TokenType.Back:
                        Log.Debug("Calling DrawLine()");
                        DrawLine(i);
                        break;

                    // LEFT and RIGHT commands move the direction the pointer is facing
                    case Token.TokenType.Left:
                        _angle += i.Num;
                        Log.Debug("Turned left " + i.Num + " degrees, _angle is now " + _angle);
                        break;
                    case Token.TokenType.Right:
                        _angle -= i.Num;
                        Log.Debug("Turned right " + i.Num + " degrees, _angle is now " + _angle);
                        break;

                    // Change the pen color
                    case Token.TokenType.Color:
                        Log.Debug("Changed pen color to " + i.Hex);
                        _penColor = i.Hex;
                        break;

                    // For a REP instruction, branch out and repeat its sub-instruction set Num times
                    case Token.TokenType.Rep:
                        for (var j = 0; j < i.Num; j++)
                            Run(i.SubInstr);
                        break;

                    // This should never be reached
                    default:
                        throw new SyntaxError(0, "Unknown instruction encountered");
                }
            }
        }

        /// <summary>
        /// Move the pointer forwards or backwards, drawing a line if the pen is down
        /// </summary>
        private void DrawLine(Instruction i)
        {
            var x1 = _x;
            var y1 = _y;
            double x2, y2;
            var d = i.Num;

            // Add to the coordinates for forward instruction
            if (i.Type.Equals(Token.TokenType.Forw))
            {
                x2 = x1 + d * Math.Cos(Math.PI * _angle / 180);
                y2 = y1 + d * Math.Sin(Math.PI * _angle / 180);
            }
            // And subtract for back command
            else if (i.Type.Equals(Token.TokenType.Back))
            {
                x2 = x1 - d * Math.Cos(Math.PI * _angle / 180);
                y2 = y1 - d * Math.Sin(Math.PI * _angle / 180);
            }
            // This should never be reached
            else
                x2 = y2 = 0;

            // Update the global position
            _x = x2;
            _y = y2;

            // If the pen is down, "draw" a line by printing coordinates
            if (_penDown)
                Console.WriteLine("{0} {1:0.0000} {2:0.0000} {3:0.0000} {4:0.0000}", _penColor, x1, y1, x2, y2);
        }
    }
}
