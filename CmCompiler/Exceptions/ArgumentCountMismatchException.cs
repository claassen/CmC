using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Exceptions
{
    public class ArgumentCountMismatchException : Exception
    {
        public ArgumentCountMismatchException(string functionName, int expected, int actual)
            : base("Argument count mismatch for function call: " + functionName + ". Function expecting " + expected + " arguments, call has " + actual)
        {
        }
    }
}
