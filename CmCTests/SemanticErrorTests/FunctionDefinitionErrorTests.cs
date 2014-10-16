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
    public class FunctionDefinitionErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(MissingReturnException))]
        public void MissingReturn_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, int y, int z) {
                      int temp = x + y;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedArgumentVariable_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, int y) {
                      return z;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateFunctionDefinitionException))]
        public void DuplicateFunctionDefinition_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, int y) {
                      return x;
                  }
                  int Test(int x, int y) {
                      return y; 
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateFunctionDefinitionException))]
        public void DuplicateFunctionDefinition_Test2()
        {
            CmCompiler.CompileText(
                @"int Test(int x, int y) {
                      return x;
                  }
                  export int Test(int x, int y) {
                      return y; 
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ExportUndefinedFunctionException))]
        public void ExportUndefinedFunction_Test()
        {
            CmCompiler.CompileText(
                @"export int Test(int x, int y);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(LargeReturnValuesNotSupportedException))]
        public void LargeReturnValuesNotSupportedTest()
        {
            CmCompiler.CompileText(
                @"typedef X { int x; int y; }
                  X Test(int a);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(LargeReturnValuesNotSupportedException))]
        public void LargeReturnValuesNotSupportedTest2()
        {
            CmCompiler.CompileText(
                @"typedef X { int x; int y; }
                  X Test(int a) {
                      X x;
                      x.x = 1;
                      x.y = 2;
                      return x;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void FunctionReturnTypeMismatch_Test()
        {
            CmCompiler.CompileText(
                @"byte Test() {
                      return (int)0;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void FunctionReturnTypeVoidMismatch_Test()
        {
            CmCompiler.CompileText(
                @"void Test() {
                      return 0;
                  }"
            );
        }
    }
}
