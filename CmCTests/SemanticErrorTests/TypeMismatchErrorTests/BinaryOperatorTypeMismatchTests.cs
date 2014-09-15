using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using CmC.Compiler.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.SemanticErrorTests.TypeMismatchErrorTests
{
    [TestClass]
    public class BinaryOperatorTypeMismatchTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchMultiplicative_Test()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  bool b;
                  int res = x * b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchMultiplicative_Test2()
        {
            CmCompiler.CompileText(
                @"bool a;
                  bool b;
                  int res = a * b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchMultiplicative_Test3()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  int y = 1;
                  bool b;
                  int res = x * y / b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchMultiplicative_Test4()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  int y = 1;
                  bool b;
                  int res = b * x / y;"
            );
        }

        [TestMethod]
        public void TypeMismatchAdditive_Test()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  bool b;
                  int res = x + b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAdditive_Test2()
        {
            CmCompiler.CompileText(
                @"bool a;
                  bool b;
                  int res = a + b;"
            );
        }

        [TestMethod]
        public void TypeMismatchAdditive_Test3()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  int y = 1;
                  bool b;
                  int res = x + y - b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAdditive_Test4()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  int y = 1;
                  bool b;
                  int res = b + x - y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchEquality_Test()
        {
            CmCompiler.CompileText(
                @"int x = 0;
                  bool b;
                  bool res = x == b;"
            );
        }
    }
}
