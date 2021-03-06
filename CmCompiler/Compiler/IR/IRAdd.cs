
using System;
using CmC.Compiler.Architecture;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRAdd : IRInstruction
    {
        public string To;
        public string Left;
        public string Right;

        public override byte[] GetImplementation(IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return String.Format("add {0} + {1} -> {2}", Left, Right, To);
        }
    }
}