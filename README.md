# Brainfuck

My Brainfuck implementation (currently written in C#).

TODO: Rewrite the project in C or Rust to simplify the project. C# is kinda
bloated.

## Supported Architectures

- [x] Windows X64;
- [x] Windows X32;
- [ ] Linux X64;
- [ ] Linux X32;
- [ ] ARM.

## Dependencies

- .NET 5 (can be downgraded);
- [NASM](https://www.nasm.us/) (_optional_; if `NASM` is defined);
- [Go Linker](http://www.godevtool.com/#linker) (_optional_; if `GO_LINK`
  is defined).

## Quick Start

1. Choose (hardcode) your architecture.

2. Build the solution and copy executables (and their dependencies: `dll`s,
   `deps.json`s, `runtimeconfig.json`s (_bruh_)) into the root of the project.

3. Create "Hello, world!" source.

   ```console
   $ echo >> hello-world.bf "++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++"
   $ echo >> hello-world.bf ".>+.+++++++..+++.>++.<<+++++++++++++++.>.+++."
   $ echo >> hello-world.bf "------.--------.>+.>."
   ```

4. Interpret or compile the source.

   -
     ```console
     $ bfi hello-world.bf
     Hello, world!
     ```
   
   - (if `NASM` and `GO_LINK` are defined)
     ```console
     $ bfc hello-world.bf
     $ ./hello-world
     Hello, world!
     ```

   - (unless `NASM` and `GO_LINK` are defined)
     ```console
     $ bfc hello-world.bf
     $ <doing something with hello-world.asm>
     ```
