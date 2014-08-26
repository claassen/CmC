﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC;
using CmC.Exceptions;
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
            var compiler = new CmCompiler();

            compiler.Compile(
                @"int x = y + 1;"
            );
        }

        [TestMethod]
        [ExpectedException(typeof(UndefinedVariableException))]
        public void UndefinedVariable_Test2()
        {
            var compiler = new CmCompiler();

            compiler.Compile(
                @"x = 1;"
            );
        }
    }
}
