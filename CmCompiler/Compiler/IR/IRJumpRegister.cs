
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRJumpRegister : IRInstruction
    {
        public string Address;

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jmp " + Address;
        }
    }
}