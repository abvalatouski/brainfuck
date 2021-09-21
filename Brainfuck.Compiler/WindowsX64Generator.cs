using System;
using System.IO;

namespace Brainfuck.Compiler
{
    // Used registers:
    // - R12 - standart input;
    // - R13 - standart output;
    // - R14 - memory pointer.
    //
    // References:
    // - Windows X64 calling convention:
    //   https://docs.microsoft.com/en-us/cpp/build/x64-calling-convention?view=msvc-160
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
    public sealed class WindowsX64Generator : IAssemblyGenerator
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

            output.WriteLine("    mov ecx, -10");
            output.WriteLine("    call GetStdHandle");
            output.WriteLine("    mov r12, rax");
            output.WriteLine();

            output.WriteLine("    mov ecx, -11");
            output.WriteLine("    call GetStdHandle");
            output.WriteLine("    mov r13, rax");
            output.WriteLine();

            output.WriteLine("    xor r14, r14");
            output.WriteLine();

            for (var i = 0; i < program.Instructions.Count; i++)
            {
                output.WriteLine("  .L{0}:", i.ToString(localLabelFormat));

                switch (program.Instructions[i])
                {
                    case '>':
                        output.WriteLine("    inc r14");
                        break;

                    case '<':
                        output.WriteLine("    dec r14");
                        break;

                    case '+':
                        output.WriteLine("    inc byte [r14 + memory]");
                        break;

                    case '-':
                        output.WriteLine("    dec byte [r14 + memory]");
                        break;

                    case '.':
                        output.WriteLine("    mov rcx, r13");
                        output.WriteLine("    lea rdx, [r14 + memory]");
                        output.WriteLine("    mov r8d, 1");
                        output.WriteLine("    mov r9, temporary");
                        output.WriteLine("    push qword 0");
                        output.WriteLine("    call WriteConsoleA");
                        break;

                    case ',':
                        output.WriteLine("    mov rcx, r12");
                        output.WriteLine("    lea rdx, [r14 + memory]");
                        output.WriteLine("    mov r8d, 1");
                        output.WriteLine("    mov r9, temporary");
                        output.WriteLine("    push qword 0");
                        output.WriteLine("    call ReadConsoleA");
                        break;

                    case '[':
                        int closeBracket = program.OpenCloseBracketPairs[i];
                        output.WriteLine("    cmp byte [r14 + memory], 0");
                        output.WriteLine("    jz .L{0}", (closeBracket + 1).ToString(localLabelFormat));
                        break;

                    case ']':
                        int openBracket = program.CloseOpenBracketPairs[i];
                        output.WriteLine("    cmp byte [r14 + memory], 0");
                        output.WriteLine("    jnz .L{0}", openBracket.ToString(localLabelFormat));
                        break;
                }

                output.WriteLine();
            }

            output.WriteLine("    mov rcx, r12");
            output.WriteLine("    call CloseHandle");
            output.WriteLine();

            output.WriteLine("    mov rcx, r13");
            output.WriteLine("    call CloseHandle");
            output.WriteLine();

            output.WriteLine("    xor ecx, ecx");
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
