namespace Brainfuck.Compiler
{
    public interface IAssembler
    {
        void Assemble(string assemblyPath, string objectPath);
    }
}
