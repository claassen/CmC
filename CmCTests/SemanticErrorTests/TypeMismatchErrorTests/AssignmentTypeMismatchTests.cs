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
                @"bool b;
                  int x = 0;
                  x = b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchAssignmentFromExpression_Test()
        {
            CmCompiler.CompileText(
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
            CmCompiler.CompileText(
                @"bool b;
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
                  X x;
                  Y y;
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
                  X x;
                  Y y;
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
                  X x;
                  Y y;
                  x.x = y.x;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test()
        {
            CmCompiler.CompileText(
                @"int y = 0;
                  int x;
                  x = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test2()
        {
            CmCompiler.CompileText(
                @"int y = 0;
                  int* x;
                  x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test3()
        {
            CmCompiler.CompileText(
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
            CmCompiler.CompileText(
                @"int* x;
                  int y;
                  y = *(*x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevelAssignment_Test5()
        {
            CmCompiler.CompileText(
                @"int* x;
                  int* y;
                  y = &x;"
            );
        }
    }
}
