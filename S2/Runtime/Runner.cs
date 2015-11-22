using System;
using System.Collections.Generic;

namespace S2
{
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

        private void Run(List<Instruction> tree)
        {
            foreach (var i in tree)
            {
                switch (i.Type)
                {
                    case Token.TokenType.UP:
                        Log.Debug("Pen is now UP");
                        _penDown = false;
                        break;
                    case Token.TokenType.DOWN:
                        Log.Debug("Pen is now DOWN");
                        _penDown = true;
                        break;
                    case Token.TokenType.FORW:
                    case Token.TokenType.BACK:
                        Log.Debug("Calling DrawLine()");
                        DrawLine(i);
                        break;
                    case Token.TokenType.LEFT:
                        _angle += i.Num;
                        Log.Debug("Turned left " + i.Num + " degrees, _angle is now " + _angle);
                        break;
                    case Token.TokenType.RIGHT:
                        _angle -= i.Num;
                        Log.Debug("Turned right " + i.Num + " degrees, _angle is now " + _angle);
                        break;
                    case Token.TokenType.COLOR:
                        Log.Debug("Changed pen color to " + i.Hex);
                        _penColor = i.Hex;
                        break;
                    case Token.TokenType.REP:
                        for (var j = 0; j < i.Num; j++)
                            Run(i.SubInstr);
                        break;
                    default:
                        throw new SyntaxError("Unknown instruction encountered");
                }
            }
        }

        private void DrawLine(Instruction i)
        {
            var x1 = _x;
            var y1 = _y;
            double x2, y2;
            var d = i.Num;

            if (i.Type.Equals(Token.TokenType.FORW))
            {
                x2 = x1 + d * Math.Cos(Math.PI * _angle / 180);
                y2 = y1 + d * Math.Sin(Math.PI * _angle / 180);
            }
            else if (i.Type.Equals(Token.TokenType.BACK))
            {
                x2 = x1 - d * Math.Cos(Math.PI * _angle / 180);
                y2 = y1 - d * Math.Sin(Math.PI * _angle / 180);
            }
            else
                x2 = y2 = 0;

            _x = x2;
            _y = y2;

            if (_penDown)
            {
                Console.WriteLine("{0} {1:0.0000} {2:0.0000} {3:0.0000} {4:0.0000}", _penColor, x1, y1, x2, y2);
            }
        }
    }
}
