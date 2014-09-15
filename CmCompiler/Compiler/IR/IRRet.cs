using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRRet : IRInstruction
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "ret";
        }
    }
}
