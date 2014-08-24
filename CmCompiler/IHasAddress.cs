using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC
{
    public interface IHasAddress
    {
        void EmitAddress(CompilationContext context);
    }
}
