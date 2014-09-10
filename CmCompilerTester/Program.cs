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

            //try
            //{
            //    CmCompiler.Compile(
            //        @"int x; int y = x = 1;",
            //        @"C:\share\array.o"
            //    );
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

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


//            CmCompiler.Compile(
//                @"int main() {
//                      int x = 1;
//                      int y = 2;
//                      int z = x + y;
//                      return 0;
//                  }", @"C:\VM\test.o", new VMArchitecture()
//            );

            CreateBIOS();
            CreateBootLoader();
            
        }

        static void CreateBIOS()
        {
            //BIOS
            var bios = new List<IRInstruction>()
            {
                //Initialize BIOS stack to address 512
                new IRMoveImmediate() { To = "bp", Value = new ImmediateValue(4096) },
                new IRMoveImmediate() { To = "sp", Value = new ImmediateValue(4096) },
                //Set IDTPointer to 1024 (start of RAM)
                new IRMoveImmediate() { To = "idt", Value = new ImmediateValue(1024) },
                //Set up IDT

                //Copy bootloader to RAM
                new IRMoveImmediate() { To = "eax", Value = new ImmediateValue(2048) },
                new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(0) },
                new IRDiskRead() { To = "eax", From = "ebx", Length = new ImmediateValue(512) },
                new IRJumpImmediate() { Address = new ImmediateValue(2048) }
            };

            CmCompiler.Compile(bios, new Dictionary<string, Function>(), @"C:\VM\rom.o", new VMArchitecture());
            CmLinker.Link(new List<string>() { @"C:\VM\rom.o" }, @"C:\VM\VM.rom", false);
        }

        static void CreateBootLoader()
        {
            CmCompiler.Compile(
                @"export int kernelinit() 
                  { 
                      return 0; 
                  }
                  byte* s = ""abcdefgh"";",
                @"C:\VM\bootloaderLib.o",
                new VMArchitecture()
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

            CmCompiler.Compile(bootLoader, bootLoaderFunctions, @"C:\VM\bootloader.o", new VMArchitecture());
            CmLinker.Link(new List<string>() { @"C:\VM\bootloader.o", @"C:\VM\bootloaderLib.o" }, @"C:\VM\bootloader.exe", false, 2048);
        }
    }
}
