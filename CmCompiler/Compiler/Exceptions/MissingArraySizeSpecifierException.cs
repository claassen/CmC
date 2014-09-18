using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class MissingArraySizeSpecifierException : Exception
    {
        public MissingArraySizeSpecifierException(string variable)
            : base("Missing array size specififier for declaration of variable: " + variable)
        {
        }
    }
}
