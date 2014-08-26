using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Context
{
    public class Function
    {
        public AbsoluteAddressValue Address;
        public ExpressionType ReturnType;
        public List<ExpressionType> ParameterTypes;
    }
}
