using System;
using CmC;
using CmC.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests
{
    [TestClass]
    public class SemanticErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MissingReturn_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"Test(x, y, z) {
                    var temp = x + y;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedVariable_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"var x = y + 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedArgumentVariable_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"Test(x, y) {
                      return z;
                  }"
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
        public void FunctionCallBeforeDefinition_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"Test(0);
                  Test(x) {
                      return 0;
                  }"
            );
        }
    }
}
