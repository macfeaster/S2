// SyntaxError.cs
// Part of the KTH course DD1361 Programming Paradigms lab S2
// Authors: Alice Heavey and Mauritz Zachrisson

using System;

namespace S2
{
    /// <summary>
    /// Represents a SyntaxError exception, thrown when any invalid token, or combination of tokens, is encountered.
    /// Displays additional error information when DEBUG constant is defined.
    /// </summary>
    [Serializable]
    internal class SyntaxError : Exception
    {
        #if DEBUG
            public SyntaxError(int line, string message) : base("Syntaxfel på rad " + line + ": " + message) {}
        #else
            public SyntaxError(int line, string message) : base("Syntaxfel på rad " + line) {}
        #endif
    }
}