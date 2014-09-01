
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRPop : IRInstruction
    {
        public string To;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public string Display()
        {
            return "pop -> " + To;
        }
    }
}