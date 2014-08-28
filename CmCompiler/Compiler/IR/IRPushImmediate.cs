using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRPushImmediate : IRInstruction, IRelocatableAddressValue
    {
        public ImmediateValue Value;

        public bool HasRelocatableAddressValue()
        {
            return Value is LabelAddressValue;
        }

        public int GetLabelIndex()
        {
            return ((LabelAddressValue)Value).Number;
        }

        public byte[] GetImplementation(Architecture.IArchitecture arch)
        {
            return arch.Implement(this);
        }
    }
}