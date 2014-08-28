
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRLoadRegister : IRInstruction
    {
        public string From;
        public string To;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}