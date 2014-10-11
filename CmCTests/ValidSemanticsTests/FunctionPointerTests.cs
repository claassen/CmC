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
    public class FunctionPointerTests
    {
        [TestMethod]
        public void VariableDeclaration_Test()
        {
            CmCompiler.CompileText(
                @"(int)(int) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationMultipleParameters_Test()
        {
            CmCompiler.CompileText(
                @"(int)(int,int,byte*) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationPointer_Test()
        {
            CmCompiler.CompileText(
                @"(int*)(int*) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationArray_Test()
        {
            CmCompiler.CompileText(
                @"(int[])(int[]) fp;"
            );
        }

        [TestMethod]
        public void VariableDeclarationNested_Test()
        {
            CmCompiler.CompileText(
                @"(int)((int)(int)) fp;"
            );
        }

        [TestMethod]
        public void Assignment_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, byte* y) { return 0; }
                  (int)(int, byte*) fp = Test;
                 "
            );
        }

        [TestMethod]
        public void Call_Test()
        {
            CmCompiler.CompileText(
                @"int Test(int x, byte* y) { return 0; }
                  (int)(int, byte*) fp = Test;
                  
                  int x;
                  byte* b;
                  x = Test(0, b);
                  //fp(x, b);
                 "
            );
        }
    }
}
