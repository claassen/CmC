using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class VoidAssignmentException : Exception
    {
        public VoidAssignmentException(string fromOrTo)
            : base("Cannot assign " + fromOrTo + "expression of type 'void'")
        {
        }
    }
}
