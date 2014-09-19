using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
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
            
//            CmCompiler.CompileText(
//                @"int test() { return 0; }
//                  int* p = &test;"
//            );
                        
            CreateBIOS();
            CreateBootLoader();
        }

        static void CreateBIOS()
        {
            CmCompiler.CompileFile(@"C:\VM\cmlib\cmlib.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmCompiler.CompileFile(@"C:\VM\bios.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmLinker.Link(new List<string>() { @"C:\VM\bios.o", @"C:\VM\cmlib\cmlib.o" }, @"C:\VM\bios.exe", false);
        }

        static void CreateBootLoader()
        {
            CmCompiler.CompileText(
                @"byte* s = ""abcdefghijklmnopqrstuvwxyz"";",
                @"C:\VM\bootloader.o",
                "",
                new RIVMArchitecture()
            );

            int loadAddress = 0; //?

            CmLinker.Link(new List<string>() { @"C:\VM\bootloader.o" }, @"C:\VM\bootloader.exe", false, loadAddress);
        }
    }
}
