using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.Architecture;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.ValidSemanticsTests
{
    [TestClass]
    public class IndirectionLevelTests
    {
        [TestMethod]
        public void IndirectionLevels_Test()
        {
            CmCompiler.CompileText(
                @"static int x = 0;
                  static int* y = &x;
                  static int** z = &y;"
            );
        }

        [TestMethod]
        public void IndirectionLevels_Test2()
        {
            CmCompiler.CompileText(
                @"static int** x;
                  static int y = *(*x);"
            );
        }

        [TestMethod]
        public void IndirectionLevelsAssignment_Test()
        {
            CmCompiler.CompileText(
                @"static int x = 0;
                  static int* y;
                  y = &x;
                  static int** z;
                  z = &y;"
            );
        }

        [TestMethod]
        public void IndirectionLevelsAssingment_Test2()
        {
            CmCompiler.CompileText(
                @"static int** x;
                  static int y;
                  y = *(*x);"
            );
        }
    }
}
