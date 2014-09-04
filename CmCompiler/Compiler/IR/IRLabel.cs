
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRLabel : IRInstruction
    {
        public int Index;

        public IRLabel(int index)
        {
            Index = index;
        }

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return new byte[0];
        }

        public override string Display()
        {
            return "LABEL " + Index + ": ";
        }
    }
}
