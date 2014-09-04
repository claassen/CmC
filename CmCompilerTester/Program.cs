using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.Architecture;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;
using CmC.Linker;

namespace CmCompilerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: type sizes - equality expression (additive, multiplicative, boolean and bitwise only support 4 byte operands)
            //TODO: return values on stack - Not supported for now, can pass pointer arguments to accomplish the same thing

//            CmCompiler.Compile(
//                @"int x = 3 - 2 - 1;",
//                @"C:\share\array.o"
//            );

//            CmCompiler.Compile(
//                @"typedef X { int x; int y; };
//                  X x;
//                  X[10] a;
//                  a[0] = x;",
//                @"C:\share\array.o"
//            );




//            CmCompiler.Compile(
//                @"export int x;
//                  extern int y;
//                  y = 1;",
//                @"C:\share\x.o"
//            );

//            CmCompiler.Compile(
//                @"extern int x;
//                  export int y;
//                  x = 2;",
//                @"C:\share\y.o"
//            );

//            CmLinker.Link(new List<string>()
//            {
//                @"C:\share\x.o",
//                @"C:\share\y.o"
//            }, @"C:\share\xy.exe", true);


            CmCompiler.Compile(
                @"int main() {
                      int x = 1;
                      int y = 2;
                      int z = x + y;
                      return 0;
                  }", @"C:\VM\test.o", new VMArchitecture()
            );

            CmLinker.Link(new List<string>() { @"C:\VM\test.o" }, @"C:\VM\test.exe", true);
        }
    }
}
