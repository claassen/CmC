using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class ExportUndefinedFunctionException : Exception
    {
        public ExportUndefinedFunctionException(string name)
            : base("Cannot export function without definition")
        {
        }
    }
}
