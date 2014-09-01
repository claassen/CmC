using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Exceptions
{
    public class UnresolvedExternalSymbolException : Exception
    {
        public UnresolvedExternalSymbolException(string name)
            : base("Unresolved external symbol: " + name)
        {
        }
    }
}
