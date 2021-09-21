using System;
using System.Collections.Generic;
using System.Text;

namespace Brainfuck
{
    public sealed class Parser
    {
        private readonly string _sourcePath;
        private readonly string _source;
        private List<int> _lineIndices;
        private int _line;
        private int _column;
        private List<(int Position, int Line, int Column)> _openingBrackets;
        private Dictionary<int, int> _openCloseBracketPairs;
        private Dictionary<int, int> _closeOpenBracketPairs;
        private List<char> _program;
        private bool _hasErrors;

        public Parser(string sourceName, string source)
        {
            _sourcePath = sourceName;
            _source = source;
        }

        public bool TryParse(out Program program)
        {
            _lineIndices           = new List<int>() { 0 };
            _line                  = 0;
            _column                = 0;
            _openingBrackets       = new List<(int Position, int Line, int Column)>();
            _openCloseBracketPairs = new Dictionary<int, int>();
            _closeOpenBracketPairs = new Dictionary<int, int>();
            _program               = new List<char>();
            _hasErrors             = false;

            for (var i = 0; i < _source.Length; i++)
            {
                switch (_source[i])
                {
                    case ' ':
                        _column++;
                        break;

                    case '\n':
                        _line++;
                        _column = 0;
                        _lineIndices.Add(i + 1);
                        break;

                    case '\t':
                        throw new NotImplementedException(
                            "Tabulation is not yet handled.");

                    case '\r':
                        break;

                    case '>':
                    case '<':
                    case '+':
                    case '-':
                    case '.':
                    case ',':
                        // This is a valid character.
                        // Do nothing for know.
                        _column++;
                        _program.Add(_source[i]);
                        break;

                    case '[':
                        _openingBrackets.Add((i, _line, _column));
                        _column++;
                        _program.Add(_source[i]);
                        break;

                    case ']':
                        if (_openingBrackets.Count != 0)
                        {
                            (int j, _, _) = _openingBrackets[^1];
                            _openingBrackets.RemoveAt(_openingBrackets.Count - 1);
                            _openCloseBracketPairs[j] = i;
                            _closeOpenBracketPairs[i] = j;
                        }
                        else
                        {
                            Error(_line, _column, "unmatched bracket");
                        }
                        _column++;
                        _program.Add(_source[i]);
                        break;

                    default:
                        Error(_line, _column, "unknown character");
                        _column++;
                        break;
                }
            }

            foreach ((int Position, int Line, int Column) in _openingBrackets)
            {
                Error(Line, Column, "unmatched bracket");
            }

            if (!_hasErrors)
            {
                program = new Program(
                    _program,
                    _openCloseBracketPairs,
                    _closeOpenBracketPairs);
                return true;
            }
            else
            {
                program = null;
                return false;
            }
        }

        //             |
        // line-number | line-contents
        //             |   ^ message
        private void Error(int line, int column, string message)
        {
            _hasErrors = true;

            string lineAsString = (line + 1).ToString();
            var buffer = new StringBuilder();

            buffer.Append(_sourcePath);
            buffer.Append(':');
            buffer.AppendLine();

            buffer.Append(' ', lineAsString.Length);
            buffer.Append(" |");
            buffer.AppendLine();

            buffer.Append(lineAsString);
            buffer.Append(" | ");
            int i = _lineIndices[line];
            do
            {
                char c = _source[i];
                if (c == '\n')
                {
                    break;
                }

                buffer.Append(c);
                i++;
            }
            while (i < _source.Length);
            buffer.AppendLine();

            buffer.Append(' ', lineAsString.Length);
            buffer.Append(" | ");
            buffer.Append(' ', column);
            buffer.Append("^ ");
            buffer.Append(message);
            buffer.AppendLine();

            Console.WriteLine(buffer.ToString());
        }
    }
}
