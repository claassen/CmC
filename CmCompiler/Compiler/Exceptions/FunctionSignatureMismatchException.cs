using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class FunctionSignatureMismatchException : Exception
    {
        public FunctionSignatureMismatchException(string functionName)
            : base("Function signature does not match previous declaration for function: " + functionName)
        {
        }
    }
}
