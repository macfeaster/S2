using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static S2.Token;

namespace S2
{
    class Instruction
    {
        public TokenType type { get; private set; }
        public int num { get; private set; }
        public string hex { get; private set; }
        public List<Instruction> subInstr { get; private set; }

        /// <summary>
        /// UP and DOWN instruction containers.
        /// </summary>
        /// <param name="type"></param>
        public Instruction(TokenType type)
        {
            this.type = type;
        }

        public Instruction(TokenType type, int num)
        {
            this.type = type;
            this.num = num;
        }


    }
}
