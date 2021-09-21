using System;
using System.IO;

namespace Brainfuck.Compiler
{
    // NOT TESTED!
    //
    // Used registers:
    // - RBX - memory pointer.
    //
    // References:
    // - Linux X64 system calls:
    //   https://blog.rchapman.org/posts/Linux_System_Call_Table_for_x86_64/
    public sealed class Linux64Generator : IAssemblyGenerator
    {
        public void GenerateAssembly(Program program, int memoryCapacity, StreamWriter output)
        {
            int digitsInInstructionsCount = (int)Math.Ceiling(Math.Log10(program.Instructions.Count));
            string localLabelFormat = $"D{digitsInInstructionsCount}";

            output.WriteLine("global _start");
            output.WriteLine();

            output.WriteLine("section .text");
            output.WriteLine();

            output.WriteLine("_start:");
            output.WriteLine();

            output.WriteLine("    xor rbx, rbx");
            output.WriteLine();

            for (var i = 0; i < program.Instructions.Count; i++)
            {
                output.WriteLine("  .L{0}:", i.ToString(localLabelFormat));

                switch (program.Instructions[i])
                {
                    case '>':
                        output.WriteLine("    inc rbx");
                        break;

                    case '<':
                        output.WriteLine("    dec rbx");
                        break;

                    case '+':
                        output.WriteLine("    inc byte [rbx + memory]");
                        break;

                    case '-':
                        output.WriteLine("    dec byte [rbx + memory]");
                        break;

                    case '.':
                        output.WriteLine("    mov rax, 1");
                        output.WriteLine("    xor rdi, rdi");
                        output.WriteLine("    lea rsi, [rbx + memory]");
                        output.WriteLine("    mov rdx, 1");
                        output.WriteLine("    syscall");
                        break;

                    case ',':
                        output.WriteLine("    xor rax, rax");
                        output.WriteLine("    xor rdi, rdi");
                        output.WriteLine("    lea rsi, [rbx + memory]");
                        output.WriteLine("    mov rdx, 1");
                        output.WriteLine("    syscall");
                        break;

                    case '[':
                        int closeBracket = program.OpenCloseBracketPairs[i];
                        output.WriteLine("    cmp byte [rbx + memory], 0");
                        output.WriteLine("    jz .L{0}", (closeBracket + 1).ToString(localLabelFormat));
                        break;

                    case ']':
                        int openBracket = program.CloseOpenBracketPairs[i];
                        output.WriteLine("    cmp byte [rbx + memory], 0");
                        output.WriteLine("    jnz .L{0}", (openBracket).ToString(localLabelFormat));
                        break;
                }
            }

            output.WriteLine("section .bss");
            output.WriteLine();

            output.WriteLine("memory:");
            output.WriteLine("    resb {0}", memoryCapacity);
        }
    }
}
