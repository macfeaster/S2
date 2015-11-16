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
        
        private void Gateway()
        {
            Token t = tokens.NextToken();

            switch (t.type)
            {
                case TokenType.BACK:
                    HandleBACK();
                    Gateway();
                    break;
                case TokenType.FORW:
                    HandleFORW();
                    Gateway();
                    break;
                case TokenType.UP:
                    HandleUP();
                    Gateway();
                    break;
                case TokenType.DOWN:
                    HandleDOWN();
                    Gateway();
                    break;
                case TokenType.LEFT:
                    HandleLEFT();
                    Gateway();
                    break;
                case TokenType.RIGHT:
                    HandleRIGHT();
                    Gateway();
                    break;
                case TokenType.COLOR:
                    HandleCOLOR();
                    Gateway();
                    break;
                case TokenType.WHITESPACE:
                    Gateway();
                    break;
            }
        }

        private void HandleBACK()
        {

        }

        private void HandleFORW()
        {

        }

        private void HandleRIGHT()
        {
            throw new NotImplementedException();
        }

        private void HandleLEFT()
        {
            throw new NotImplementedException();
        }

        private void HandleDOWN()
        {
            throw new NotImplementedException();
        }

        private void HandleUP()
        {
            throw new NotImplementedException();
        }

        private void HandleCOLOR()
        {
            throw new NotImplementedException();
        }

       
    }
}
