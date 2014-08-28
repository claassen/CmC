
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRMult : IRInstruction
    {
        public string To;
        public string Left;
        public string Right;

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}