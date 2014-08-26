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
    public class FunctionDefinitionErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(MissingReturnException))]
        public void MissingReturn_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int Test(int x, int y, int z) {
                      int temp = x + y;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedArgumentVariable_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int Test(int x, int y) {
                      return z;
                  }"
            );
        }
    }
}
