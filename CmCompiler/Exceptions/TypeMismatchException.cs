using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Exceptions
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(Type t1, Type t2)
            : base("Mismatched types: " + t1 + " and " + t2)
        {
        }
    }
}
