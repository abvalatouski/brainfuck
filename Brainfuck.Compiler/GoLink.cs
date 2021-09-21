using System.Diagnostics;

namespace Brainfuck.Compiler
{
    public sealed class GoLink : ILinker
    {
        private readonly string _entryPoint;
        private readonly string[] _dependencies;

        public GoLink(string entryPoint, string[] dependencies)
        {
            _entryPoint = entryPoint;
            _dependencies = dependencies;
        }

        public void Link(string objectPath, string executablePath)
        {
            var info = new ProcessStartInfo(
                "golink.exe",
                $"\"{objectPath}\" {string.Join(" ", _dependencies)} /console /entry {_entryPoint} >nul");
            info.UseShellExecute = true;
            Process.Start(info);
        }
    }
}
