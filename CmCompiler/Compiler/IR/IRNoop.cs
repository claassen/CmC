
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRNoop : IRInstruction
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "noop";
        }
    }
}