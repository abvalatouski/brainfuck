using System.Diagnostics;

namespace Brainfuck.Compiler
{
    public sealed class Nasm : IAssembler
    {
        private readonly string _objectFormat;

        public Nasm(string objectFormat)
        {
            _objectFormat = objectFormat;
        }

        public void Assemble(string assemblyPath, string objectPath)
        {
            Process.Start(
                "nasm.exe",
                $"\"{assemblyPath}\" -f {_objectFormat} -o \"{objectPath}\"");
        }
    }
}
