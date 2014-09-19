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
    public class LiteralValueTests
    {
        [TestMethod]
        public void DecimalNumberLiteral_Test()
        {
            CmCompiler.CompileText(
                @"int x = 42a;"
            );
        }

        [TestMethod]
        public void HexNumberLiteral_Test()
        {
            CmCompiler.CompileText(
                @"int x = 0x000001EF;"
            );
        }

        [TestMethod]
        public void StringLiteral_Test()
        {
            CmCompiler.CompileText(
                @"byte* s = ""abcdef"";"
            );
        }
    }
}
