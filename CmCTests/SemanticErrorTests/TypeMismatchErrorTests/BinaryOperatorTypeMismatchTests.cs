using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC;
using CmC.Exceptions;
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
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  bool b;
                  int res = x * b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchMultiplicative_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool a;
                  bool b;
                  int res = a * b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchMultiplicative_Test3()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
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
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int y = 1;
                  bool b;
                  int res = b * x / y;"
            );
        }

        [TestMethod]
        public void TypeMismatchAdditive_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  bool b;
                  int res = x + b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAdditive_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool a;
                  bool b;
                  int res = a + b;"
            );
        }

        [TestMethod]
        public void TypeMismatchAdditive_Test3()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
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
            var compiler = new CmCompiler();

            compiler.Compile(
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
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  bool b;
                  bool res = x == b;"
            );
        }
    }
}
