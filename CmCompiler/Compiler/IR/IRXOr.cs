
using System;
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRXOr : IRInstruction
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
            return String.Format("xor {0}, {1} -> {2}", Left, Right, To);
        }
    }
}