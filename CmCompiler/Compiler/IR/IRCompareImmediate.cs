using System;
using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRCompareImmediate : IRInstruction
    {
        public string Left;
        public ImmediateValue Right;

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return String.Format("cmp {0}, {1}", Left, Right);
        }
    }
}