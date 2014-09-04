using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
{
    public class ImmediateValue
    {
        public int Value;

        public ImmediateValue(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public abstract class AddressValue : ImmediateValue
    {
        public AddressValue(int offset)
            : base(offset)
        {
        }
    }

    public class StackAddressValue : AddressValue
    {
        public string BaseRegister;

        public StackAddressValue(int offset)
            : base(offset)
        {
            BaseRegister = "bp";
        }

        public override string ToString()
        {
            return "bp + " + Value;
        }
    }

    public class LabelAddressValue : AddressValue
    {
        public LabelAddressValue(int label)
            : base(label)
        {
        }

        public override string ToString()
        {
            return "LABEL" + Value;
        }
    }
}
