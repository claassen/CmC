
namespace CmC.Compiler.IR
{
    public class IRJumpLT : IRJumpImmediate
    {
        public new byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jl " + Address;
        }
    }
}