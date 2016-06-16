using CmC.Compiler;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
using CmC.Linker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RIVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmCTests.RIVMTests
{
    [TestClass]
    public class TestBase
    {
        public TestVM ExecuteTestVM(string testCode)
        {
            CompilationContext context = CmCompiler.CompileText(testCode);

            byte[] objectCode;

            using (var s = new MemoryStream())
            {
                CmCompiler.CreateObjectCode(s, new RIVMArchitecture(), context.GetIR(), context.GetStringConstants(), context.GetGlobalVariables(), context.GetFunctions());
                objectCode = s.GetBuffer();
            }

            byte[] executableCode;

            using (var s = new MemoryStream())
            {
                CmLinker.Link(s, new List<byte[]> { objectCode }, false, SystemMemoryMap.BIOS_ROM_START);
                executableCode = s.GetBuffer();
            }

            var bios = new BIOS(executableCode);

            var mmu = new MMU(SystemMemoryMap.BIOS_STACK_END, bios, null, null);

            var cpu = new CPU(mmu);

            cpu.Start();

            return new TestVM(cpu);
        }

        public int StartingBasePointer()
        {
            return SystemMemoryMap.BIOS_STACK_START;
        }

        public int StartingInstructionAddress()
        {
            return SystemMemoryMap.BIOS_ROM_START;
        }

        public int StackOffsetForFunctionCall(int sizeOfArgs)
        {
            return 4 + //base pointer
                   sizeOfArgs +
                   4;  //return address;
        }
    }

    public class TestVM
    {
        private CPU _cpu;

        public TestVM(CPU cpu)
        {
            _cpu = cpu;
        }

        public void AssertStackOffsetValue(int offset, int value, int size)
        {
            Assert.AreEqual(
                value, 
                BitHelper.ExtractBytes(_cpu.Memory.Get(_cpu.Registers[RIVM.Register.BP] + offset, false, size), size)
            );
        }

        public void AssertBasePointerOffset(int offset)
        {
            Assert.AreEqual(offset, _cpu.Registers[RIVM.Register.BP] - SystemMemoryMap.BIOS_STACK_START);
        }

        public void AssertStackPointerOffset(int offset)
        {
            Assert.AreEqual(offset, _cpu.Registers[RIVM.Register.SP] - SystemMemoryMap.BIOS_STACK_START);
        }

        public void AssertValueAtMemoryByDereferencingValueAtStackOffset(int offset, int value, int size)
        {
            int stackValue = _cpu.Memory.Get(_cpu.Registers[RIVM.Register.BP] + offset, false, 4);
            int memoryValue = BitHelper.ExtractBytes(_cpu.Memory.Get(stackValue, false, size), size);
            Assert.AreEqual(value, memoryValue);
        }
    }
}
