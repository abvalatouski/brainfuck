using System.IO;

namespace Brainfuck.Compiler
{
    public interface IAssemblyGenerator
    {
        void GenerateAssembly(Program program, int memoryCapacity, StreamWriter output);
    }
}
