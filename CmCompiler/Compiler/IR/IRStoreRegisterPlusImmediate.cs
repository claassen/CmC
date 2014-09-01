using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRStoreRegisterPlusImmediate : IRInstruction
    {
        public string From;
        public string To;
        public ImmediateValue Offset;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public string Display()
        {
            return "store " + From + " -> [" + To + " + " + Offset + "]";
        }
    }
}