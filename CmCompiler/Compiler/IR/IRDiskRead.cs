using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRDiskRead : IRInstruction
    {
        public string From;
        public string To;
        public ImmediateValue Length;

        public override byte[] GetImplementation(IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "read Disk[" + From + "] -> [" + To + "] : " + Length.Value;
        }
    }
}
