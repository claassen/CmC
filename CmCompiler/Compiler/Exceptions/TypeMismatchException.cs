using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;

namespace CmC.Compiler.Exceptions
{
    public class TypeMismatchException : Exception
    {
        public TypeMismatchException(ExpressionType t1, ExpressionType t2)
            : base("Mismatched types: " + t1 + " and " + t2)
        {
        }
    }
}
