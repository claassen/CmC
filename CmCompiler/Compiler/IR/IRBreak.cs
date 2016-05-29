using CmC.Compiler.Architecture;
using CmC.Compiler.IR.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.IR
{
    public class IRBreak : IRInstruction
    {
        public override byte[] GetImplementation(IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "break";
        }
    }
}
