// Log.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

using System;

namespace S2
{
    /// <summary>
    /// Logs program internals to stdout when DEBUG constant is defined.
    /// </summary>
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
