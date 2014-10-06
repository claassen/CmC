
namespace CmC.Compiler.IR
{
    public class IRJumpGT : IRJumpImmediate
    {
        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jg " + Address;
        }
    }
}