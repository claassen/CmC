using CmC.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmCTests.ParsingTests.ValidSemanticsTests
{
    [TestClass]
    public class FunctionCallTests
    {
        [TestMethod]
        public void FunctionCallWithPrimaryExpressionArgument_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x) {
                      return x;
                  }
                  Test((1 + 2) * 3);
                "
            );
        }
    }
}
