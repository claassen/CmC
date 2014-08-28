using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class UndefinedFunctionException : Exception
    {
        public UndefinedFunctionException(string name)
            : base("Undefined function: " + name)
        {
        }
    }
}
