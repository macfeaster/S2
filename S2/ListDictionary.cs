using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
    class ListDictionary : SortedDictionary<int, List<Token>>
    {
        public void Add(int lineNumber, Token token)
        {
            List<Token> existing;

            if (this.TryGetValue(lineNumber, out existing))
            {
                existing = new List<Token>();
                this[lineNumber] = existing;
            }

            existing.Add(token);
        }
    }
}
