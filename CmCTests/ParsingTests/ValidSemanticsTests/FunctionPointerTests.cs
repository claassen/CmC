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
    public class FunctionPointerTests
    {
        [TestMethod]
        public void VariableDeclaration_Test()
        {
            CmCompiler.CompileText(
                @"static (int)(int) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationMultipleParameters_Test()
        {
            CmCompiler.CompileText(
                @"static (int)(int,int,byte*) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationPointer_Test()
        {
            CmCompiler.CompileText(
                @"static (int*)(int*) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationArray_Test()
        {
            CmCompiler.CompileText(
                @"static (int[])(int[]) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationNested_Test()
        {
            CmCompiler.CompileText(
                @"static (int)((int)(int)) fp;"
            );
        }

        [TestMethod]
        public void Assignment_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, byte* y) { return 0; }
                  static (int)(int, byte*) fp = Test;
                 "
            );
        }

        [TestMethod]
        public void Call_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, byte* y) { return 0; }
                  static (int)(int, byte*) fp = Test;
                  
                  static byte* b;
                  static int x = Test(0, b);
                  static int y = fp(x, b);
                 "
            );
        }

        [TestMethod]
        public void ArrayOfFunctionPointers_Test()
        {
            CmCompiler.CompileText(
                @"static (int)(int,int)[10] fpArray;"
            );
        }
    }
}
