using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;

namespace CmCompilerTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var compiler = new CmCompiler();

            //TODO: arrays
            //TODO: type definitions, type sizes
            //TODO: return values on stack

            byte[] data = compiler.Compile(
                @"int x = 0;
                  int y = 1;
                  if(x == y) {
                      x = 1;
                  }
                  else {
                      if(x < y) {
                          x = 2;
                      }
                      else {
                          x = 3;
                      }
                  }"
            );

//            compiler.Compile(
//                @"int x;
//                  int* y = &x;
//                  *y = 5;"
//            );

//            compiler.Compile(
//                @"typedef X 
//                  { 
//                      int x; 
//                      int y; 
//                  };
//                  typedef Y
//                  {
//                      int a;
//                      X* px;
//                  };
//                  int a;
//                  Y y;
//                  y.px = &a;"
//            );
        }

        static void PrintIR(List<IRInstruction> ir)
        {
            //Console.WriteLine("data:");
            //int addr = 0;
            //foreach (var v in _globalVarSymbolTable)
            //{
            //    Console.WriteLine(addr++ + ": " + v.Value + ": " + v.Key);
            //}

            Console.WriteLine("code:");
            foreach (var i in ir)
            {
                //if(!(i is Comment))
                Console.WriteLine(i);
            }
        }
    }
}
