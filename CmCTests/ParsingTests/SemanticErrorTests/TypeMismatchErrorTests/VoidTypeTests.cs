using CmC.Compiler;
using CmC.Compiler.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmCTests.SemanticErrorTests.TypeMismatchErrorTests
{
    [TestClass]
    public class VoidTypeTests
    {
        [TestMethod]
        public void BasicVoidTypeDeclaration_Test()
        {
            CmCompiler.CompileText(
                @"static void v;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(VoidAssignmentException))]
        public void VoidDefinitionAssignmentTo_Test()
        {
            CmCompiler.CompileText(
                @"static void a = (void)0;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(VoidAssignmentException))]
        public void VoidAssignmentTo_Test()
        {
            CmCompiler.CompileText(
                @"static void a;
                  static void b;
                  a = b;"
            );
        }
    }
}
