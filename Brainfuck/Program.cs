using System.Collections.Generic;

namespace Brainfuck
{
    public sealed class Program
    {
        public Program(
            IReadOnlyList<char> instructions,
            Dictionary<int, int> openCloseBracketPairs,
            Dictionary<int, int> closeOpenBracketPairs)
        {
            Instructions          = instructions;
            OpenCloseBracketPairs = openCloseBracketPairs;
            CloseOpenBracketPairs = closeOpenBracketPairs;
        }

        public IReadOnlyList<char> Instructions { get; }

        public Dictionary<int, int> OpenCloseBracketPairs { get; }

        public Dictionary<int, int> CloseOpenBracketPairs { get; }
    }
}
