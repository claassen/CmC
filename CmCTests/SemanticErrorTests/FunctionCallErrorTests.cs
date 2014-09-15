using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.SemanticErrorTests
{
    [TestClass]
    public class FunctionCallErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentCountMismatchException))]
        public void FunctionArgumentCountMismatch_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, int y, int z) {
                      return x + y + z;
                  }
                  Test(1, 2);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void FunctionArgumentTypeMismatch_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, int y) {
                      return x + y;
                  }
                  bool b;
                  Test(b, 2);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedFunctionException))]
        public void UndefinedFunction_Test()
        {
            CmCompiler.CompileText(
                @"NotTest();"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedFunctionException))]
        public void UndefinedFunctionCallBeforeDefinition_Test()
        {
            CmCompiler.CompileText(
                @"Test(0);
                  int Test(int x) {
                      return 0;
                  }"
            );
        }
    }
}
