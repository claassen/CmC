using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class DuplicateFunctionDefinitionException : Exception
    {
        public DuplicateFunctionDefinitionException(string name)
            : base("Function " + name + " is already defined")
        {
        }
    }
}
