using System;

namespace S2
{
    internal static class Log
    {
        public static void Debug(string message)
        {
            #if DEBUG
                Console.WriteLine(message);
            #endif
        }
    }
}
