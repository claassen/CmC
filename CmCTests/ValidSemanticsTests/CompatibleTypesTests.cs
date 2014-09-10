using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.ValidSemanticsTests
{
    [TestClass]
    public class CompatibleTypesTests
    {
        [TestMethod]
        public void CompatibleTypeBoolean_Test()
        {
            CmCompiler.Compile(
                @"int x = 0;
                  bool b;
                  bool res = x && b;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test2()
        {
            CmCompiler.Compile(
                @"int x = 0;
                  int y = 1;
                  bool res = x && y;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test3()
        {
            CmCompiler.Compile(
                @"bool a;
                  bool b;
                  bool res = a && b ^ 5;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test4()
        {
            CmCompiler.Compile(
                @"bool a;
                  bool b;
                  bool res = 5 || a ^ b;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test5()
        {
            CmCompiler.Compile(
                @"int a;
                  int b;
                  bool res = a < 5 && b != 1;"
            );
        }
    }
}
