
namespace CmC.Compiler.IR
{
    public class IRJumpLT : IRJumpImmediate
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jl " + Address;
        }
    }
}