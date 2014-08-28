using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
{
    public class Variable
    {
        public AddressValue Address;
        public ExpressionType Type;

        public override string ToString()
        {
            return Type.ToString();
        }
    }
}
