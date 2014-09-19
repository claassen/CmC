using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;

namespace CmC.Compiler.Syntax.Common.Interface
{
    public interface IHasValue
    {
        ImmediateValue GetValue(CompilationContext context);
    }
}
