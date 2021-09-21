using System;
using System.IO;

namespace Brainfuck.Interpreter
{
    public static class EntryPoint
    {
        private const int ExitSuccess = 0;
        private const int ExitFailure = 1;

        internal static int Main()
        {
            string[] arguments = Environment.GetCommandLineArgs();
            string programName = arguments[0];
            if (arguments.Length != 2)
            {
                PrintUsage(programName);
                return ExitFailure;
            }

            string sourceFile = arguments[1];
            if (!File.Exists(sourceFile))
            {
                Console.Error.WriteLine("cannot open the file '{0}'", sourceFile);
                return ExitFailure;
            }

            string sourceContents = File.ReadAllText(sourceFile);
            var parser = new Parser(sourceFile, sourceContents);
            if (!parser.TryParse(out Program program))
            {
                Console.Error.WriteLine("terminating due to previous errors");
                return ExitFailure;
            }

            Interpret(program);
            return ExitSuccess;
        }

        private static void Interpret(Program program)
        {
            const int MemoryCapacity = 30000;
            var memory = new byte[MemoryCapacity];
            var memoryPosition = 0;

            var instructionPointer = 0;
            while (instructionPointer < program.Instructions.Count)
            {
                switch (program.Instructions[instructionPointer])
                {
                    case '>':
                        memoryPosition++;
                        instructionPointer++;
                        break;

                    case '<':
                        memoryPosition--;
                        instructionPointer++;
                        break;

                    case '+':
                        memory[memoryPosition]++;
                        instructionPointer++;
                        break;

                    case '-':
                        memory[memoryPosition]--;
                        instructionPointer++;
                        break;

                    case '.':
                        Console.Write((char)memory[memoryPosition]);
                        instructionPointer++;
                        break;

                    case ',':
                        memory[memoryPosition] = (byte)Console.Read();
                        instructionPointer++;
                        break;

                    case '[':
                        if (memory[memoryPosition] == 0)
                        {
                            int closeBracket = program.OpenCloseBracketPairs[instructionPointer];
                            instructionPointer = closeBracket + 1;
                        }
                        else
                        {
                            instructionPointer++;
                        }
                        break;

                    case ']':
                        if (memory[memoryPosition] != 0)
                        {
                            int openBracket = program.CloseOpenBracketPairs[instructionPointer];
                            instructionPointer = openBracket;
                        }
                        else
                        {
                            instructionPointer++;
                        }
                        break;
                }
            }
        }

        private static void PrintUsage(string programName)
        {
            Console.Error.WriteLine("Usage: {0} <FILE>", programName);
        }
    }
}
