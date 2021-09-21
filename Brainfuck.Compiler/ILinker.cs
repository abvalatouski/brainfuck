namespace Brainfuck.Compiler
{
    public interface ILinker
    {
        void Link(string objectPath, string executablePath);
    }
}
