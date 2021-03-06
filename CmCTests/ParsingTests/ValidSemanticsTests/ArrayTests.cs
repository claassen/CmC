﻿using System;
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
                @"static int[10] x;"
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
                @"static int[10] x;
                  x[1] = 1;"
            );
        }

        [TestMethod]
        public void ArrayOfPointersAssignment_Test()
        {
            CmCompiler.CompileText(
                @"static int x; static int y;
                  static int*[10] a;
                  a[0] = &x; a[1] = &y;"
            );
        }

        [TestMethod]
        public void PointerAsArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"static int x; static int y;
                  static int* a;
                  a[0] = x; a[1] = y;"
            );
        }

        [TestMethod]
        public void PointerAsArrayAssignment_Test2()
        {
            CmCompiler.CompileText(
                @"static int x; static int y;
                  static int** a;
                  a[0] = &x; a[1] = &y;"
            );
        }

        [TestMethod]
        public void ArrayOfPointersAsMultiDimensionalArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"static int x; static int y;
                  static int*[10] a;
                  (a[0])[0] = x; 
                  (a[1])[1] = y;"
            );
        }

        [TestMethod]
        public void ArrayToArrayAssignment_Test()
        {
            CmCompiler.CompileText(
                @"static int[10] x;
                  static int[5] y = x;"
            );
        }

        [TestMethod]
        public void AddressOfArrayIsSameAsArray_Test()
        {
            CmCompiler.CompileText(
                @"static int[10] x;
                  static int* p = x;
                  static int* q = &x;"
            );
        }

        [TestMethod]
        public void ArrayFunctionArgument_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int[] a) { return 0; }"
            );
        }

        [TestMethod]
        public void ArrayFunctionArgumentCalledWithArray_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int[] a) { return 0; }
                  static int[10] a;
                  Test(a);"
            );
        }

        [TestMethod]
        public void ArrayFunctionArgumentCalledWithPointer_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int[] a) { return 0; }
                  static int* a;
                  Test(a);"
            );
        }
    }
}
