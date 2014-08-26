using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC;
using CmC.Exceptions;
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
            var compiler = new CmCompiler();

            compiler.Compile(
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
            var compiler = new CmCompiler();

            compiler.Compile(
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
            var compiler = new CmCompiler();

            compiler.Compile(
                @"NotTest();"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedFunctionException))]
        public void UndefinedFunctionCallBeforeDefinition_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"Test(0);
                  int Test(int x) {
                      return 0;
                  }"
            );
        }
    }
}
