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
    public class AssignmentTypeMismatchTests
    {
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
        public void TypeMismatchCustomTypeAssignment_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  };
                  typedef Y
                  {
                      int a;
                      X* px;
                  };
                  X x;
                  Y y;
                  y = x;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeFieldAssignment_Test()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  };
                  typedef Y
                  {
                      int a;
                      X* px;
                  };
                  X x;
                  Y y;
                  x.x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeFieldAssignment_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  };
                  typedef Y
                  {
                      int a;
                      X x;
                  };
                  X x;
                  Y y;
                  x.x = y.x;"
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
        public void TypeMismatchIndirectionLevelAssignment_Test5()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int* x;
                  int* y;
                  y = &x;"
            );
        }
    }
}
