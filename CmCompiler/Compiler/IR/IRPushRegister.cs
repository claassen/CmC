
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRPushRegister : IRInstruction
    {
        public string From;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}