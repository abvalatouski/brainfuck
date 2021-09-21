using System.Diagnostics;

namespace Brainfuck.Compiler
{
    public sealed class WindowsNetwideAssembler : IAssembler
    {
        public void Assemble(string assemblyPath, string objectPath)
        {
            Process.Start("nasm.exe", $"\"{assemblyPath}\" -f win64 -o \"{objectPath}\"");
        }
    }
}
