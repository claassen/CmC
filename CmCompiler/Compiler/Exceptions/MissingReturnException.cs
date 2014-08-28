using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class MissingReturnException : Exception
    {
        public MissingReturnException(string funcName)
            : base("Missing return statement in function: " + funcName)
        {
        }
    }
}
