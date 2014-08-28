using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class UndefinedVariableException : Exception
    {
        public UndefinedVariableException(string name)
            : base("Undefined variable: " + name)
        {
        }
    }
}
