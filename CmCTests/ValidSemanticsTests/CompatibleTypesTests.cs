using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.ValidSemanticsTests
{
    [TestClass]
    public class CompatibleTypesTests
    {
        [TestMethod]
        public void CompatibleTypeBoolean_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  bool b;
                  bool res = x && b;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int y = 1;
                  bool res = x && y;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test3()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool a;
                  bool b;
                  bool res = a && b ^ 5;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test4()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool a;
                  bool b;
                  bool res = 5 || a ^ b;"
            );
        }
    }
}
