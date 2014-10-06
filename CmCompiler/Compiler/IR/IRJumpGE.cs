
namespace CmC.Compiler.IR
{
    public class IRJumpGE : IRJumpImmediate
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jge " + Address;
        }
    }
}