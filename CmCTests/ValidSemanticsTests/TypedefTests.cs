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
    public class TypedefTests
    {
        [TestMethod]
        public void FieldAssignment_Test()
        {
            CmCompiler.CompileText(
                @"typedef X { int x; int y; }
                  X myX;
                  myX.x = 0;
                  myX.y = 1;"
            );
        }

        [TestMethod]
        public void FieldAssignmentNested_Test()
        {
            CmCompiler.CompileText(
                @"typedef X { int x; int y; }
                  typedef Y { X x; }
                  Y myY;
                  (myY.x).x = 0;"
            );
        }

        [TestMethod]
        public void PointerFieldAssignment_Test()
        {
            CmCompiler.CompileText(
                @"typedef X { int x; int y; }
                  X* myX;
                  myX->x = 0;
                  myX->y = 1;"
            );
        }

        [TestMethod]
        public void PointerFieldAssignmentNested_Test()
        {
            CmCompiler.CompileText(
                @"typedef X { int x; int y; }
                  typedef Y { X* x; }
                  Y* myY;
                  (myY->x)->x = 0;"
            );
        }
    }
}
