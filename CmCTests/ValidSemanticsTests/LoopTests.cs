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
    public class LoopTests
    {
        [TestMethod]
        public void BasicForLoop_Test()
        {
            CmCompiler.CompileText(
                @"for(int i = 0; i < 10; i = i + 1)
                  {
                      int j = i;
                  }"
            );
        }
    }
}
