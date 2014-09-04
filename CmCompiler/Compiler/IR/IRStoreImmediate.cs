using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRStoreImmediate : IRInstruction
    {
        public string From;
        public ImmediateValue To;

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "store " + From + " -> [" + To + "]";
        }
    }
}