using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
    class Runner
    {
        bool penDown = false;
        string penColor = "#0000FF";
        double x = 0;
        double y = 0;
        int angle = 0;

        public Runner(List<Instruction> tree)
        {
            Run(tree);
        }

        public void Run(List<Instruction> tree)
        {
            foreach (Instruction i in tree)
            {
                switch (i.type)
                {
                    case Token.TokenType.UP:
                        Log.Debug("Pen is now UP");
                        penDown = false;
                        break;
                    case Token.TokenType.DOWN:
                        Log.Debug("Pen is now DOWN");
                        penDown = true;
                        break;
                    case Token.TokenType.FORW:
                    case Token.TokenType.BACK:
                        Log.Debug("Calling DrawLine()");
                        DrawLine(i);
                        break;
                    case Token.TokenType.LEFT:
                        angle += i.num;
                        Log.Debug("Turned left " + i.num + " degrees, angle is now " + angle);
                        break;
                    case Token.TokenType.RIGHT:
                        angle -= i.num;
                        Log.Debug("Turned right " + i.num + " degrees, angle is now " + angle);
                        break;
                    case Token.TokenType.COLOR:
                        Log.Debug("Changed pen color to " + i.hex);
                        penColor = i.hex;
                        break;
                    case Token.TokenType.REP:
                        for (int j = 0; j < i.num; j++)
                            Run(i.subInstr);
                        break;
                    default:
                        throw new SyntaxError("Unknown instruction encountered");
                }
            }
        }

        public void DrawLine(Instruction i)
        {
            double x1 = x;
            double y1 = y;
            double x2, y2;
            int d = i.num;

            if (penDown)
            {
                if (i.type.Equals(Token.TokenType.FORW))
                {
                    x2 = x1 + d * Math.Cos(Math.PI * angle / 180);
                    y2 = y1 + d * Math.Sin(Math.PI * angle / 180);
                }
                else if (i.type.Equals(Token.TokenType.BACK))
                {
                    x2 = x1 - d * Math.Cos(Math.PI * angle / 180);
                    y2 = y1 - d * Math.Sin(Math.PI * angle / 180);
                }
                else
                    x2 = y2 = 0;

                Console.WriteLine(
                    string.Format(
                        "{0} {1:0.0000} {2:0.0000} {3:0.0000} {4:0.0000}",
                        penColor, x1, y1, x2, y2
                    )
                );

                x = x2;
                y = y2;
            }
        }
    }
}
