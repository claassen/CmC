
using System;
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRDiv : IRInstruction
    {
        public string To;
        public string Left;
        public string Right;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public string Display()
        {
            return String.Format("div {0} / {1} -> {2}", Left, Right, To);
        }
    }
}