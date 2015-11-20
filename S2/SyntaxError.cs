using System;

namespace S2
{
    [Serializable]
    internal class SyntaxError : Exception
    {
        // public SyntaxError(int line) : base("Syntaxfel på rad " + line) {}

        public SyntaxError(int line, string message) : base("Syntaxfel på rad " + line + ": " + message) {}

        public SyntaxError(string message) : base(message) {}
    }
}