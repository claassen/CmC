using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRJumpImmediate : IRInstruction, IRelocatableAddressValue
    {
        public ImmediateValue Address;

        public bool HasRelocatableAddressValue()
        {
            return Address is LabelAddressValue;
        }

        public int GetLabelIndex()
        {
            return ((LabelAddressValue)Address).Value;
        }

        public override byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "jmp " + Address;
        }
    }
}