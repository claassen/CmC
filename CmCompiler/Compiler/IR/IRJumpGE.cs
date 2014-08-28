
namespace CmC.Compiler.IR
{
    public class IRJumpGE : IRJumpImmediate
    {
        public new byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}