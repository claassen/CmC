
using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRLoadImmediate : IRInstruction, IRelocatableAddressValue
    {
        public string To;
        public ImmediateValue Address;

        public bool HasRelocatableAddressValue()
        {
            return Address is LabelAddressValue;
        }

        public int GetLabelIndex()
        {
            return ((LabelAddressValue)Address).Number;
        }

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}
