
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRShiftLeft : IRInstruction
    {
        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }


        public string Display()
        {
            throw new System.NotImplementedException();
        }
    }
}