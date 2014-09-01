using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;
using CmC.Linker;

namespace CmCompilerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: arrays
            //TODO: type sizes - Done?
            //TODO: return values on stack - Not supported for now, can pass pointer arguments to accomplish the same thing

            CmCompiler.Compile(
                @"export int x;
                  extern int y;
                  y = 1;",
                @"C:\share\x.o"
            );

            CmCompiler.Compile(
                @"extern int x;
                  export int y;
                  x = 2;",
                @"C:\share\y.o"
            );

            CmLinker.Link(new List<string>()
            {
                @"C:\share\x.o",
                @"C:\share\y.o"
            }, @"C:\share\xy.exe");
        }
    }
}
