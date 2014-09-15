using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;

namespace CmC.Compiler.IR.Interface
{
    public abstract class IRInstruction
    {
        public abstract byte[] GetImplementation(IArchitecture arch);
        public abstract string Display();
        public int OperandSize = 4;
    }
}
