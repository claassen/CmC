using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class LargeReturnValuesNotSupportedException : Exception
    {
        public LargeReturnValuesNotSupportedException()
            : base("Return values larger than 4 bytes are not yet supported")
        {
        }
    }
}
