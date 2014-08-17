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
                @"var test = Avg(2, 4); 
                  var x = 1 * 2 + 3;
                  var y = 2 + 3 * 4;
                  Avg(x, y) {
                      return (x + y) / 2;
                  }
                  Avg(x, y);"
            );
        }
    }
}
