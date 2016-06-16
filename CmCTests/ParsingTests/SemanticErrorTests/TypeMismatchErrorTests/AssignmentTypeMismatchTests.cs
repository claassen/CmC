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
    public class AssignmentTypeMismatchTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromVariable_Test()
        {
            CmCompiler.CompileText(
                @"static bool b;
                  static int x = 0;
                  x = b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromExpression_Test()
        {
            CmCompiler.CompileText(
                @"static int x = 0;
                  static int y = 1;
                  static bool b;
                  b = x + y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromNumber_Test()
        {
            CmCompiler.CompileText(
                @"static bool b;
                  b = 5;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeAssignment_Test()
        {
            CmCompiler.CompileText(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  }
                  typedef Y
                  {
                      int a;
                      X* px;
                  }
                  static X x;
                  static Y y;
                  y = x;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeFieldAssignment_Test()
        {
            CmCompiler.CompileText(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  }
                  typedef Y
                  {
                      int a;
                      X* px;
                  }
                  static X x;
                  static Y y;
                  x.x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeFieldAssignment_Test2()
        {
            CmCompiler.CompileText(
                @"typedef X 
                  { 
                      int x; 
                      int y; 
                  }
                  typedef Y
                  {
                      int a;
                      X x;
                  }
                  static X x;
                  static Y y;
                  x.x = y.x;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test()
        {
            CmCompiler.CompileText(
                @"static int y = 0;
                  static int x;
                  x = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test2()
        {
            CmCompiler.CompileText(
                @"static int y = 0;
                  static int* x;
                  x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test3()
        {
            CmCompiler.CompileText(
                @"static int x = 0;
                  static int *y;
                  y = &x;
                  static int *z;
                  z = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test4()
        {
            CmCompiler.CompileText(
                @"static int* x;
                  static int y;
                  y = *(*x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test5()
        {
            CmCompiler.CompileText(
                @"static int* x;
                  static int* y;
                  y = &x;"
            );
        }
    }
}
