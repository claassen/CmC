
using System;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRMult : IRInstruction
    {
        public string To;
        public string Left;
        public string Right;

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return String.Format("mult {0} * {1} -> {2}", Left, Right, To);
        }
    }
}