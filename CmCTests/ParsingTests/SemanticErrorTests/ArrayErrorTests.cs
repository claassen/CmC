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
    public class ArrayErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void ArrayIndexOnNonArray_Test()
        {
            CmCompiler.CompileText(
                @"static int x;
                  x[1] = 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void ArrayIndexOnNonArray_Test2()
        {
            CmCompiler.CompileText(
                @"static int[10] x;
                  (x[1])[2] = 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void ArrayOfPointersAssignmentError_Test()
        {
            CmCompiler.CompileText(
                @"static int x; static int y;
                  static int*[10] a;
                  a[0] = x; a[1] = y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void PointerAsArrayAssignmentError_Test()
        {
            CmCompiler.CompileText(
                @"static int x; static int y;
                  static int* a;
                  a[0] = &x; a[1] = &y;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void AssignmentToArrayVariable_Test()
        {
            CmCompiler.CompileText(
                @"static int[10] a;
                  a = 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(TypeMismatchException))]
        public void AssignmentToArrayVariable_Test2()
        {
            CmCompiler.CompileText(
                @"static int[10] a;
                  static int[10] b;
                  a = b;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(MissingArraySizeSpecifierException))]
        public void MissingArraySizeSpecifier_Test()
        {
            CmCompiler.CompileText(
                @"static int[] a;"
            );
        }
    }
}
