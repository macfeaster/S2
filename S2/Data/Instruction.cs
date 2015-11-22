using System.Collections.Generic;
using System.Text;

namespace S2
{
    internal class Instruction
    {
        public Token.TokenType Type { get; private set; }
        public int Num { get; private set; }
        public string Hex { get; private set; }
        public List<Instruction> SubInstr { get; private set; }

        /// <summary>
        /// UP and DOWN instruction containers.
        /// </summary>
        /// <param name="type"></param>
        public Instruction(Token.TokenType type)
        {
            Type = type;
        }

        public Instruction(Token.TokenType type, int num)
        {
            Type = type;
            Num = num;
        }

        public Instruction(Token.TokenType type, string hex)
        {
            Type = type;
            Hex = hex;
        }

        public Instruction(Token.TokenType type, int num, List<Instruction> subInstr)
        {
            Type = type;
            Num = num;
            SubInstr = subInstr;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("<").Append(Type);

            if (Num > 0)
                sb.Append(" ").Append(Num);

            if (!string.IsNullOrEmpty(Hex))
                sb.Append(" ").Append(Hex);

            if (SubInstr != null)
                sb
                    .Append(@"  [")
                    .Append(string.Join(", ", SubInstr))
                    .Append("]");

            sb.Append(">");

            return sb.ToString();
        }

    }
}
