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
    public class DefinitionTypeMismatchTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionFromVariable_Test()
        {
            CmCompiler.CompileText(
                @"static bool b;
                  static int x = b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionFromNumber_Test()
        {
            CmCompiler.CompileText(
                @"static bool b = 5;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionFromExpression_Test()
        {
            CmCompiler.CompileText(
                @"static int x = 0;
                  static int y = 1;
                  static bool b = x + y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeDefinition_Test()
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
                  static Y y = x;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchCustomTypeDefinitionIndirectionLevel_Test()
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
                  static Y y;
                  static X x = y.px;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionIndirectionLevel_Test()
        {
            CmCompiler.CompileText(
                @"static int y = 0;
                  static int x = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionIndirectionLevel_Test2()
        {
            CmCompiler.CompileText(
                @"static int y = 0;
                  static int* x = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchDefinitionIndirectionLevel_Test3()
        {
            CmCompiler.CompileText(
                @"static int* x;
                  static int y = *(*x);"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void TypeMismatchIndirectionLevel_Test4()
        {
            CmCompiler.CompileText(
                @"static int* x;
                  static int* y = &x;"
            );
        }
    }
}
