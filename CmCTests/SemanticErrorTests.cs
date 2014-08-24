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
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevel_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int y = 0;
                  int x = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int y = 0;
                  int x;
                  x = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevel_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int y = 0;
                  int* x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int y = 0;
                  int* x;
                  x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevel_Test3()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int* y = &x;
                  int* z = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test3()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int *y;
                  y = &x;
                  int *z;
                  z = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevel_Test4()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int* x;
                  int y = *(*x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test4()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int* x;
                  int y;
                  y = *(*x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevel_Test5()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int* x;
                  int* y = &x;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test5()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int* x;
                  int* y;
                  y = &x;"
            );
        }

        [TestMethod]
        public void IndirectionLevels_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int* y = &x;
                  int** z = &y;"
            );
        }

        [TestMethod]
        public void IndirectionLevelsAssignment_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int* y;
                  y = &x;
                  int** z;
                  z = &y;"
            );
        }

        [TestMethod]
        public void IndirectionLevels_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int** x;
                  int y = *(*x);"
            );
        }

        [TestMethod]
        public void IndirectionLevelsAssingment_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int** x;
                  int y;
                  y = *(*x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionFromVariable_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool b;
                  int x = b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionFromNumber_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool b = 5;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionFromExpression_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int y = 1;
                  bool b = x + y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromVariable_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool b;
                  int x = 0;
                  x = b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromExpression_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int y = 1;
                  bool b;
                  b = x + y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromNumber_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool b;
                  b = 5;"
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

        [TestMethod]
        public void CompatibleTypeBoolean_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  bool b;
                  bool res = x && b;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = 0;
                  int y = 1;
                  bool res = x && y;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test3()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool a;
                  bool b;
                  bool res = a && b ^ 5;"
            );
        }

        [TestMethod]
        public void CompatibleTypeBoolean_Test4()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"bool a;
                  bool b;
                  bool res = 5 || a ^ b;"
            );
        }

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
        [ExpectedException(typeof(TypeMismatchException))]
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
        [ExpectedException(typeof(TypeMismatchException))]
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
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedVariable_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = y + 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedVariable_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"x = 1;"
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
                  int Test(int x) {
                      return 0;
                  }"
            );
        }
    }
}
