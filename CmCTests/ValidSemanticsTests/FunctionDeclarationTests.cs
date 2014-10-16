using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.ValidSemanticsTests
{
    [TestClass]
    public class FunctionDeclarationTests
    {
        [TestMethod]
        public void DeclarationBeforeDefinitionTest()
        {
            CmCompiler.CompileText(
                @"int Test(int x);
                  int Test(int x) {
                      return x;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateFunctionDefinitionException))]
        public void DeclarationAfterDefinitionTest()
        {
            CmCompiler.CompileText(
                @"int Test(int x) {
                      return x;
                  }
                  int Test(int x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(FunctionSignatureMismatchException))]
        public void DefinitionDoesNotMatchDeclarationTest()
        {
            CmCompiler.CompileText(
                @"int Test(int x);
                  int Test(int x, int y) {
                      return 0;
                  }"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(FunctionSignatureMismatchException))]
        public void MultipleDeclarationsSignatureMismatchTest()
        {
            CmCompiler.CompileText(
                @"int Test(int x);
                  int Test(int x, int y) {
                      return 0;
                  }"
            );
        }

        [TestMethod]
        public void VoidFunctionNoReturn_Test()
        {
            CmCompiler.CompileText(
                @"void Test() {
                      int x = 0;
                  }"
            );
        }

        [TestMethod]
        public void VoidFunctionWithReturn_Test()
        {
            CmCompiler.CompileText(
                @"void Test() {
                      int x = 0;
                      return;
                  }"
            );
        }
    }
}
