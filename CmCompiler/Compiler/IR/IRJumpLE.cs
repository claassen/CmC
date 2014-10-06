
namespace CmC.Compiler.IR
{
    public class IRJumpLE : IRJumpImmediate
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jle " + Address;
        }
    }
}