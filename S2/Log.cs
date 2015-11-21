using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2
{
    class Log
    {
        public static void Debug(string message)
        {
            #if DEBUG
                Console.WriteLine(message);
            #endif
        }
    }
}
