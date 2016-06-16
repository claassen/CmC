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
            CmCompiler.CompileText(
                @"static int x = 0;
                  static bool b;
                  static bool res = x && b;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test2()
        {
            CmCompiler.CompileText(
                @"static int x = 0;
                  static int y = 1;
                  static bool res = x && y;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test3()
        {
            CmCompiler.CompileText(
                @"static bool a;
                  static bool b;
                  static bool res = a && b ^ 5;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test4()
        {
            CmCompiler.CompileText(
                @"static bool a;
                  static bool b;
                  static bool res = 5 || a ^ b;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test5()
        {
            CmCompiler.CompileText(
                @"static int a;
                  static int b;
                  static bool res = a < 5 && b != 1;"
            );
        }
    }
}
