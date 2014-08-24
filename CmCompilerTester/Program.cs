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
//                @"int** x;
//                  int y = *(*x);"
//            );

            compiler.Compile(
                @"int* x;
                  int y = *x;
                  int Test(int* a) {
                      *a = 42;
                      return 0;
                  }
                  Test(&y);"
            );

//            compiler.Compile(
//                @"int a = 1;
//                  int b = 2;
//                  int Test(int x, int y, int z) {
//                      int temp = 0;
//                      if(x == y) {
//                          temp = x + z;
//                      }
//                      else {
//                          temp = x + y;
//                      }
//                      return temp;
//                  }
//                  int res = Test(a, b);"
//            );

//            compiler.Compile(
//                @"var a = 1;
//                  var b = 2;
//                  Add(x, y, z) {
//                      var temp = x + y;
//                      if(x == y) {
//                          var p = temp + 1;
//                      }
//                      else if(y == z) {
//                          var p = x + 2;
//                      }
//                      else {
//                          var p = y + 3;
//                      }
//                      var tempTwo = x == y;
//                      return temp;
//                  }
//                  var res = Add(a, b);"
//            );
        }
    }
}
