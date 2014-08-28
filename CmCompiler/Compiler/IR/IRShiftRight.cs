
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRShiftRight : IRInstruction
    {
        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}