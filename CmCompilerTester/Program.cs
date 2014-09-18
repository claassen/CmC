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
            //CreateBootLoader();
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
                @"export int kernelinit() 
                  { 
                      return 0; 
                  }
                  byte* s = ""abcdefgh"";",
                @"C:\VM\bootloaderLib.o",
                new RIVMArchitecture()
            );

            
            var bootLoader = new List<IRInstruction>()
            {
                //Enter protected mode
                new IRPushImmediate() { Value = new LabelAddressValue(1) },
                new IRJumpImmediate() { Address = new LabelAddressValue(0) },
                new IRLabel(1),
                new IRMoveImmediate() { To = "cr", Value = new ImmediateValue(1) }
            };

            var bootLoaderFunctions = new Dictionary<string, Function>();

            bootLoaderFunctions.Add("kernelinit", new Function() { IsDefined = false, Address = new LabelAddressValue(0) });

            CmCompiler.CompileIR(@"C:\VM\bootloader.o", new RIVMArchitecture(), bootLoader, null, null, bootLoaderFunctions);
            CmLinker.Link(new List<string>() { @"C:\VM\bootloader.o", @"C:\VM\bootloaderLib.o" }, @"C:\VM\bootloader.exe", false, 2048);
        }
    }
}
