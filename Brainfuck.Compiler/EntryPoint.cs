using System;
using System.IO;

namespace Brainfuck.Compiler
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

            string executablePath = sourceFile.Substring(0, sourceFile.LastIndexOf('.'));
            Compile(executablePath, program);
            return ExitSuccess;
        }

        private static void Compile(string executablePath, Program program)
        {
            IAssemblyGenerator generator = new WindowsX64Generator();
            var output = new StreamWriter($"{executablePath}.asm");
            generator.GenerateAssembly(program, memoryCapacity: 30000, output);
            output.Close();

            #if NASM
            // Depends on NASM:
            // https://www.nasm.us/
            IAssembler assembler = new WindowsNetwideAssembler();
            assembler.Assemble($"{executablePath}.asm", $"{executablePath}.obj");
            #endif

            #if GO_LINK
            // Depends on GoLink:
            // http://www.godevtool.com/#linker
            ILinker linker = new WindowsGoLinker();
            linker.Link($"{executablePath}.obj", $"{executablePath}.exe");
            #endif
        }

        private static void PrintUsage(string programName)
        {
            Console.Error.WriteLine("Usage: {0} <FILE>", programName);
        }
    }
}
