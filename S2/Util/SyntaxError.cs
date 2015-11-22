using System;

namespace S2
{

    [Serializable]
    internal class SyntaxError : Exception
    {
        #if DEBUG
            public SyntaxError(int line, string message) : base("Syntaxfel på rad " + line + ": " + message) {}
        #else
            public SyntaxError(int line, string message) : base("Syntaxfel på rad " + line) {}
        #endif

        public SyntaxError(string message) : base(message) {}
    }
}