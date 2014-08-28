
namespace CmC.Compiler.IR
{
    public class IRJumpGT : IRJumpImmediate
    {
        public new byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}