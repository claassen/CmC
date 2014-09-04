
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRStoreRegister : IRInstruction
    {
        public string From;
        public string To;

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