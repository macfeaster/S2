using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static S2.Token;

namespace S2
{
    class Parser
    {
        ListDictionary tokens;
        int repDepth = 0;

        // how2entrypoint
        public Parser(ListDictionary tokens)
        {
            this.tokens = tokens;
        }

        private void Gateways()
        {
            Token t = tokens.NextToken();

            switch (t.type)
            {
                case TokenType.BACK:
                    HandleBACK();
                    break;

            }
        }

        private void HandleBACK()
        {

        }
    }
}
