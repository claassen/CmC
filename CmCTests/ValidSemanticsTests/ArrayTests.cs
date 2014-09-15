using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmCTests.ValidSemanticsTests
{
    [TestClass]
    public class ArrayTests
    {
        [TestMethod]
        public void ArrayDeclaration_Test()
        {
            CmCompiler.CompileText(
                @"int[10] x;"
            );
        }

        [TestMethod]
        public void ArrayDeclarationExport_Test()
        {
            CmCompiler.CompileText(
                @"export int[10] x;"
            );
        }

        [TestMethod]
        public void ArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"int[10] x;
                  x[1] = 1;"
            );
        }

        [TestMethod]
        public void ArrayOfPointersAssignment_Test()
        {
            CmCompiler.CompileText(
                @"int x; int y;
                  int*[10] a;
                  a[0] = &x; a[1] = &y;"
            );
        }

        [TestMethod]
        public void PointerAsArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"int x; int y;
                  int* a;
                  a[0] = x; a[1] = y;"
            );
        }

        [TestMethod]
        public void PointerAsArrayAssignment_Test2()
        {
            CmCompiler.CompileText(
                @"int x; int y;
                  int** a;
                  a[0] = &x; a[1] = &y;"
            );
        }

        [TestMethod]
        public void ArrayOfPointersAsMultiDimensionalArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"int x; int y;
                  int*[10] a;
                  (a[0])[0] = x; 
                  (a[1])[1] = y;"
            );
        }

        [TestMethod]
        public void ArrayToArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"int[10] x;
                  int[5] y = x;"
            );
        }

        [TestMethod]
        public void AddressOfArrayIsSameAsArray_Test()
        {
            CmCompiler.CompileText(
                @"int[10] x;
                  int* p = x;
                  int* q = &x;"
            );
        }
    }
}
