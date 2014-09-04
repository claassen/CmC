
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.IR
{
    public class IRMoveImmediate : IRInstruction, IRelocatableAddressValue
    {
        public string To;
        public ImmediateValue Value;

        public override byte[] GetImplementation(IArchitecture arch)
        {
            return arch.Implement(this);
        }

        public override string Display()
        {
            return "mov " + Value + " -> " + To;
        }

        public bool HasRelocatableAddressValue()
        {
            return Value is LabelAddressValue;
        }

        public int GetLabelIndex()
        {
            return ((LabelAddressValue)Value).Value;
        }
    }
}
