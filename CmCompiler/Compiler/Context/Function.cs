using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
{
    public class Function
    {
        public AddressValue Address;
        public ExpressionType ReturnType;
        public List<ExpressionType> ParameterTypes;
        public bool IsExported;
        public bool IsDefined;
        public bool IsExtern { get { return !IsDefined; } }
    }
}
