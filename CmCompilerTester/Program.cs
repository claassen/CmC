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

            CreateStdLib();
            CreateBIOS();
            CreateBootLoader();
            CreateKernel();
        }

        static void CreateStdLib()
        {
            CmCompiler.CompileFile(@"C:\VM\cmlib\stdlib.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });
            CmCompiler.CompileFile(@"C:\VM\cmlib\disk.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });
            CmCompiler.CompileFile(@"C:\VM\cmlib\graphics.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });
            CmCompiler.CompileFile(@"C:\VM\cmlib\rivm\ivt.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });
        }

        static void CreateBIOS()
        {
            CmCompiler.CompileFile(@"C:\VM\bios\bios.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmLinker.Link(
                new List<string>() 
                { 
                    @"C:\VM\bios\bios.o", 
                    @"C:\VM\cmlib\stdlib.o", 
                    @"C:\VM\cmlib\disk.o", 
                    @"C:\VM\cmlib\graphics.o",
                    @"C:\VM\cmlib\rivm\ivt.o"
                }, 
                @"C:\VM\bios.exe", 
                false, 
                0x000F0000
            );
        }

        static void CreateBootLoader()
        {
            CmCompiler.CompileFile(@"C:\VM\bootloader\bootloader.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            CmLinker.Link(new List<string>() { @"C:\VM\bootloader\bootloader.o", @"C:\VM\cmlib\disk.o", @"C:\VM\cmlib\stdlib.o" }, @"C:\VM\bootloader.exe", false, 0x00007C00);
        }

        static void CreateKernel()
        {
            //CmCompiler.CompileFile(@"C:\VM\kernel\kernel.cm", new CompilerOptions() { Architecture = new RIVMArchitecture() });

            //CmLinker.Link(new List<string>() { @"C:\VM\kernel\kernel.o", @"C:\VM\cmlib\cmlib.o" }, @"C:\VM\kernel.exe", false, 0);
        }
    }
}
