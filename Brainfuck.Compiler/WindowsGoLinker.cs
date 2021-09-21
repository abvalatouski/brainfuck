using System.Diagnostics;

namespace Brainfuck.Compiler
{
    public sealed class WindowsGoLinker : ILinker
    {
        public void Link(string objectPath, string executablePath)
        {
            var info = new ProcessStartInfo(
                "golink.exe",
                $"\"{objectPath}\" kernel32.dll /console /entry mainCRTStartup >nul")
            {
                UseShellExecute = true,
            };
            Process.Start(info);
            // Process.Start("golink.exe", $"\"{objectPath}\" kernel32.dll /console /entry mainCRTStartup");
        }
    }
}
