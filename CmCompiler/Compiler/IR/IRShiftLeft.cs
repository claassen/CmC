
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRShiftLeft : IRInstruction
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            throw new System.NotImplementedException();
        }
    }
}