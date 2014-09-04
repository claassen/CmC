
using System;
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRCompareRegister : IRInstruction
    {
        public string Left;
        public string Right;

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