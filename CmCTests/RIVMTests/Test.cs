using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmCTests.RIVMTests
{
    [TestClass]
    public class Test : TestBase
    {
        [TestMethod]
        public void StackVariableDeclaration_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    int x = 42;
                    int y = 43;

                    asm {"" halt ""}
                }
            ");

            vm.AssertStackOffsetValue(4, 42, 4);
            vm.AssertStackOffsetValue(8, 43, 4);
            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 8);
        }

        [TestMethod]
        public void VariableAssignmentFromLiteral_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    int x;
                    x = 42;

                    asm {"" halt ""}
                }
            ");

            vm.AssertStackOffsetValue(4, 42, 4);
            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 4);
        }

        [TestMethod]
        public void VariableAssignmentFromVariable_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();
                
                test();

                void test()
                {
                    int x = 42;
                    int y;
                    y = x;

                    asm {"" halt ""}
                }
            ");

            vm.AssertStackOffsetValue(4, 42, 4);
            vm.AssertStackOffsetValue(8, 42, 4);
            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 8);
        }

        [TestMethod]
        public void VariableAssignmentFromFunctionCall_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();
                int f();
                
                test();

                void test()
                {
                    int x = f();
                
                    asm {"" halt ""}
                }
                
                int f()
                {
                    return 42;
                }
            ");

            vm.AssertStackOffsetValue(4, 42, 4);
            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 4);
        }

        [TestMethod]
        public void VariableAssigmentFromExpression_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    int x = 42;
                    int y = ((x + 2) * 2) / (3 + 1) - 1;

                    asm {"" halt ""}
                }
            ");

            vm.AssertStackOffsetValue(4, 42, 4);
            vm.AssertStackOffsetValue(8, ((42 + 2) * 2) / (3 + 1) - 1, 4);
            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 8);
        }

        [TestMethod]
        public void ImplicitReturnInVoidFunction_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                }

                asm {""
                    push 0
                    push 0
                    push 0
                ""}
            ");

            vm.AssertBasePointerOffset(0);
            vm.AssertStackPointerOffset(0);
        }


        [TestMethod]
        public void FunctionCallWithArgumentsInsideFunction_Test()
        {
            var vm = ExecuteTestVM(@"
                void test(int x, int y);

                test(1, 2);

                void test(int x, int y)
                {
                    asm {"" halt ""}
                }
            ");

            vm.AssertStackOffsetValue(-12, StartingBasePointer(), 4);  //stored bp
            vm.AssertStackOffsetValue(-8, 2, 4);   //arg1
            vm.AssertStackOffsetValue(-4, 1, 4);   //arg0
            //offset 0 would be the return address from the function
            vm.AssertBasePointerOffset(12);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(8));
        }

        [TestMethod]
        public void FunctionCallWithArgumentsAfterFunctionReturn_Test()
        {
            var vm = ExecuteTestVM(@"
                void test(int x, int y);

                test(1, 2);

                void test(int x, int y)
                {
                }
            ");

            vm.AssertBasePointerOffset(0);
            vm.AssertStackPointerOffset(0);
        }



        [TestMethod]
        public void ForLoopVariableDeclaration_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();
                
                void test()
                {
                    for(int i = 0; i < 10; i = i + 1)
                    {
                    }
                
                    asm {"" halt ""}
                }
            ");

            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 4);
        }

        [TestMethod]
        public void VariableDeclarationInsideForLoop_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    for(int i = 0; i < 10; i = i + 1)
                    {
                        int x;
                    }
                
                    asm {"" halt ""}
                }
            ");

            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 8);
        }

        [TestMethod]
        public void VariableDeclarationInsideConditional_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    if(1)
                    {
                        int x;
                    }
                    else if(0)
                    {
                        int y;
                    }
                    else
                    {
                        int y;
                    }
                
                    asm {"" halt ""}
                }
            ");

            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 12);
        }

        [TestMethod]
        public void StringVariableDeclarationAsByteArray_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    byte[10] s = ""ABC"";

                    asm {"" halt ""}
                }
            ");

            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 10);
            vm.AssertStackOffsetValue(4, (int)'A', 1);
            vm.AssertStackOffsetValue(5, (int)'B', 1);
            vm.AssertStackOffsetValue(6, (int)'C', 1);
            vm.AssertStackOffsetValue(7, 0, 1);
            vm.AssertStackOffsetValue(8, 0, 1);
            vm.AssertStackOffsetValue(9, 0, 1);
            vm.AssertStackOffsetValue(10, 0, 1);
            vm.AssertStackOffsetValue(11, 0, 1);
            vm.AssertStackOffsetValue(12, 0, 1);
            vm.AssertStackOffsetValue(13, 0, 1);
        }

        [TestMethod]
        public void StringVariableDeclarationAsBytePointer_Test()
        {
            var vm = ExecuteTestVM(@"
                void test();

                test();

                void test()
                {
                    byte* s = ""ABC"";

                    asm {"" halt ""}
                }
            ");

            vm.AssertBasePointerOffset(4);
            vm.AssertStackPointerOffset(StackOffsetForFunctionCall(0) + 4);
            vm.AssertValueAtMemoryByDereferencingValueAtStackOffset(4, (int)'A', 1);
        }
    }
}
