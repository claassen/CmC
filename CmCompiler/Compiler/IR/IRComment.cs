
using CmC.Compiler.IR.Interface;
namespace CmC.Compiler.IR
{
    public class IRComment : IRInstruction
    {
        public string Message;

        public IRComment(string message)
        {
            Message = message;
        }

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return new byte[0];
        }

        public override string Display()
        {
            return ";;" + Message;
        }
    }
}
