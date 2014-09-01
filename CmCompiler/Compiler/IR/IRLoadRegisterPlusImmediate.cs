using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRLoadRegisterPlusImmediate : IRInstruction
    {
        public string To;
        public string From;
        public ImmediateValue Offset;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public string Display()
        {
            return "load [" + From + " + " + Offset + "] -> " + To;
        }
    }
}