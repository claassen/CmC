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

            compiler.Compile(
                @"var a = 1;
                  var b = 2;
                  Add(x, y, z) {
                      var temp = x + y;
                      if(x == y) {
                          var p = temp + 1;
                      }
                      else if(y == z) {
                          var p = x + 2;
                      }
                      else {
                          var p = y + 3;
                      }
                      var tempTwo = x == y;
                      return temp;
                  }
                  var res = Add(a, b);"
            );
        }
    }
}
