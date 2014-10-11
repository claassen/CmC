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
                
            CreateBIOS();
            CreateBootLoader();
            CreateKernel();
        }

        static void CreateBIOS()
        {
            CmCompiler.CompileFile(@"C:\VM\cmlib\cmlib.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmCompiler.CompileFile(@"C:\VM\rivm\bios\bios.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmLinker.Link(new List<string>() { @"C:\VM\rivm\bios\bios.o", @"C:\VM\cmlib\cmlib.o" }, @"C:\VM\bios.exe", false, 0x000F0000);
        }

        static void CreateBootLoader()
        {
            CmCompiler.CompileText(
                @"byte* s = ""abcdefghijklmnopqrstuvwxyz"";",
                @"C:\VM\bootloader.o",
                "",
                new RIVMArchitecture()
            );

            CmLinker.Link(new List<string>() { @"C:\VM\bootloader.o" }, @"C:\VM\bootloader.exe", false, 0x00007C00);
        }

        static void CreateKernel()
        {
            CmCompiler.CompileFile(@"C:\VM\kernel\kernel.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmLinker.Link(new List<string>() { @"C:\VM\kernel\kernel.o", @"C:\VM\cmlib\cmlib.o" }, @"C:\VM\kernel.exe", false, 0);
        }
    }
}
