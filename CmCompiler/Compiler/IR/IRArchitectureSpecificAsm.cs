using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRArchitectureSpecificAsm : IRInstruction
    {
        public string Instruction;

        public IRArchitectureSpecificAsm(string instruction)
        {
            Instruction = instruction;
        }

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return Instruction;
        }
    }
}
