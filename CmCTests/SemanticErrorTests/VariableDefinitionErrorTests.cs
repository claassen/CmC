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
    public class VariableDefinitionErrorTests
    {
        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedVariable_Test()
        {
            CmCompiler.Compile(
                @"int x = y + 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedVariable_Test2()
        {
            CmCompiler.Compile(
                @"x = 1;"
            );
        }
    }
}
