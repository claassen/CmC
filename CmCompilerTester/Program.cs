using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC;

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

//            compiler.Compile(
//                @"int x;
//                  int* y = &x;
//                  *y = 5;"
//            );

            compiler.Compile(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  };
                  typedef Y
                  {
                      int a;
                      X* px;
                  };
                  int a;
                  Y y;
                  y.px = &a;"
            );
        }
    }
}
