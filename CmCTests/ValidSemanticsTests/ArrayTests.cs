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
            CmCompiler.Compile(
                @"int[10] x;"
            );
        }

        [TestMethod]
        public void ArrayDeclarationExport_Test()
        {
            CmCompiler.Compile(
                @"export int[10] x;"
            );
        }

        [TestMethod]
        public void ArrayAssignment_Test()
        {
            CmCompiler.Compile(
                @"int[10] x;
                  x[1] = 1;"
            );
        }

        [TestMethod]
        public void ArrayOfPointersAssignment_Test()
        {
            CmCompiler.Compile(
                @"int x; int y;
                  int*[10] a;
                  a[0] = &x; a[1] = &y;",
                @"C:\share\array.o"
            );
        }

        [TestMethod]
        public void PointerAsArrayAssignment_Test()
        {
            CmCompiler.Compile(
                @"int x; int y;
                  int* a;
                  a[0] = x; a[1] = y;",
                @"C:\share\array.o"
            );
        }

        [TestMethod]
        public void PointerAsArrayAssignment_Test2()
        {
            CmCompiler.Compile(
                @"int x; int y;
                  int** a;
                  a[0] = &x; a[1] = &y;",
                @"C:\share\array.o"
            );
        }

        [TestMethod]
        public void ArrayOfPointersAsMultiDimensionalArrayAssignment_Test()
        {
            CmCompiler.Compile(
                @"int x; int y;
                  int*[10] a;
                  (a[0])[0] = x; 
                  (a[1])[1] = y;",
                @"C:\share\array.o"
            );
        }
    }
}
