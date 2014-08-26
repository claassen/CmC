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
    public class IndirectionLevelTests
    {
        [TestMethod]
        public void IndirectionLevels_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int* y = &x;
                  int** z = &y;"
            );
        }

        [TestMethod]
        public void IndirectionLevels_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int** x;
                  int y = *(*x);"
            );
        }

        [TestMethod]
        public void IndirectionLevelsAssignment_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int* y;
                  y = &x;
                  int** z;
                  z = &y;"
            );
        }

        [TestMethod]
        public void IndirectionLevelsAssingment_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int** x;
                  int y;
                  y = *(*x);"
            );
        }
    }
}
