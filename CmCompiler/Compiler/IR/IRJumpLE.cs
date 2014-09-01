
namespace CmC.Compiler.IR
{
    public class IRJumpLE : IRJumpImmediate
    {
        public new byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jle " + Address;
        }
    }
}