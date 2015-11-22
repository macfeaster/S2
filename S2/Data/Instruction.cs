﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
    class Instruction
    {
        public Token.TokenType type { get; private set; }
        public int num { get; private set; }
        public string hex { get; private set; }
        public List<Instruction> subInstr { get; private set; }

        /// <summary>
        /// UP and DOWN instruction containers.
        /// </summary>
        /// <param name="type"></param>
        public Instruction(Token.TokenType type)
        {
            this.type = type;
        }

        public Instruction(Token.TokenType type, int num)
        {
            this.type = type;
            this.num = num;
        }

        public Instruction(Token.TokenType type, string hex)
        {
            this.type = type;
            this.hex = hex;
        }

        public Instruction(Token.TokenType type, int num, List<Instruction> subInstr)
        {
            this.type = type;
            this.num = num;
            this.subInstr = subInstr;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<").Append(type);

            if (num > 0)
                sb.Append(" ").Append(num);

            if (!string.IsNullOrEmpty(hex))
                sb.Append(" ").Append(hex);

            if (subInstr != null)
                sb
                    .Append(@"  [")
                    .Append(string.Join(", ", subInstr))
                    .Append("]");

            sb.Append(">");

            return sb.ToString();
        }

    }
}