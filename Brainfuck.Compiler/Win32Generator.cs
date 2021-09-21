using System;
using System.IO;

namespace Brainfuck.Compiler
{
    // Used registers:
    // - ESI - standart input;
    // - EDI - standart output;
    // - EBX - memory pointer.
    //
    // References:
    // - Windows X32 calling convention:
    //   https://en.wikipedia.org/wiki/X86_calling_conventions#stdcall
    // - CloseHandle:
    //   https://docs.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle
    // - ExitProcess:
    //   https://docs.microsoft.com/en-us/windows/win32/api/processthreadsapi/nf-processthreadsapi-exitprocess
    // - GetStdHandle:
    //   https://docs.microsoft.com/en-us/windows/console/getstdhandle
    // - ReadConsole:
    //   https://docs.microsoft.com/en-us/windows/console/readconsole
    // - WriteConsole:
    //   https://docs.microsoft.com/en-us/windows/console/writeconsole
    public sealed class Win32Generator : IAssemblyGenerator
    {
        public void GenerateAssembly(Program program, int memoryCapacity, StreamWriter output)
        {
            int digitsInInstructionsCount = (int)Math.Ceiling(Math.Log10(program.Instructions.Count));
            string localLabelFormat = $"D{digitsInInstructionsCount}";

            output.WriteLine("global mainCRTStartup");
            output.WriteLine();
            output.WriteLine("extern CloseHandle");
            output.WriteLine("extern ExitProcess");
            output.WriteLine("extern GetStdHandle");
            output.WriteLine("extern ReadConsoleA");
            output.WriteLine("extern WriteConsoleA");
            output.WriteLine();

            output.WriteLine("section .text");
            output.WriteLine();

            output.WriteLine("mainCRTStartup:");
            output.WriteLine();

            output.WriteLine("    push dword -10");
            output.WriteLine("    call GetStdHandle");
            output.WriteLine("    mov esi, eax");
            output.WriteLine();

            output.WriteLine("    push dword -11");
            output.WriteLine("    call GetStdHandle");
            output.WriteLine("    mov edi, eax");
            output.WriteLine();

            output.WriteLine("    xor ebx, ebx");
            output.WriteLine();

            for (var i = 0; i < program.Instructions.Count; i++)
            {
                output.WriteLine("  .L{0}:", i.ToString(localLabelFormat));

                switch (program.Instructions[i])
                {
                    case '>':
                        output.WriteLine("    inc ebx");
                        break;

                    case '<':
                        output.WriteLine("    dec ebx");
                        break;

                    case '+':
                        output.WriteLine("    inc byte [ebx + memory]");
                        break;

                    case '-':
                        output.WriteLine("    dec byte [ebx + memory]");
                        break;

                    case '.':
                        output.WriteLine("    push dword 0");
                        output.WriteLine("    push temporary");
                        output.WriteLine("    push dword 1");
                        output.WriteLine("    lea eax, [ebx + memory]");
                        output.WriteLine("    push eax");
                        output.WriteLine("    push edi");
                        output.WriteLine("    call WriteConsoleA");
                        break;

                    case ',':
                        output.WriteLine("    push dword 0");
                        output.WriteLine("    push temporary");
                        output.WriteLine("    push dword 1");
                        output.WriteLine("    lea eax, [ebx + memory]");
                        output.WriteLine("    push eax");
                        output.WriteLine("    push edi");
                        output.WriteLine("    call ReadConsoleA");
                        break;

                    case '[':
                        int closeBracket = program.OpenCloseBracketPairs[i];
                        output.WriteLine("    cmp byte [ebx + memory], 0");
                        output.WriteLine("    jz .L{0}", (closeBracket + 1).ToString(localLabelFormat));
                        break;

                    case ']':
                        int openBracket = program.CloseOpenBracketPairs[i];
                        output.WriteLine("    cmp byte [ebx + memory], 0");
                        output.WriteLine("    jnz .L{0}", openBracket.ToString(localLabelFormat));
                        break;
                }

                output.WriteLine();
            }

            output.WriteLine("    push esi");
            output.WriteLine("    call CloseHandle");
            output.WriteLine();

            output.WriteLine("    push edi");
            output.WriteLine("    call CloseHandle");
            output.WriteLine();

            output.WriteLine("    push dword 0");
            output.WriteLine("    call ExitProcess");
            output.WriteLine();

            output.WriteLine("section .bss");
            output.WriteLine();

            output.WriteLine("temporary:");
            output.WriteLine("    resd 1");

            output.WriteLine("memory:");
            output.WriteLine("    resb {0}", memoryCapacity);
        }
    }
}
