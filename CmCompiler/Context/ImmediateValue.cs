using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Context
{
    public class ImmediateValue
    {
        public int Number;

        public ImmediateValue(int num)
        {
            Number = num;
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }

    public abstract class AddressValue : ImmediateValue
    {
        public AddressValue(int offset)
            : base(offset)
        {
        }
    }

    public class AbsoluteAddressValue : AddressValue
    {
        public AbsoluteAddressValue(int offset)
            : base(offset)
        {
        }
    }

    public class StaticAddressValue : AddressValue
    {
        public StaticAddressValue(int offset)
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
            return "[bp + " + Number + "]";
        }
    }

    public class LabelAddressValue : AddressValue
    {
        public string Label;

        public LabelAddressValue(string labelName)
            : base(0)
        {
            Label = labelName;
        }
    }
}
