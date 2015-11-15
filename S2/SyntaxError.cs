using System;

namespace S2
{
    [Serializable]
    internal class SyntaxError : Exception
    {
        public SyntaxError(int lineCount) : base("Syntaxfel på rad " + lineCount) {}

        public SyntaxError(string message) : base(message) {}
    }
}