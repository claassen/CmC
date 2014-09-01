using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;

namespace CmC.Compiler.Syntax.TokenInterfaces
{
    public interface IHasType
    {
        ExpressionType GetExpressionType(CompilationContext context);
    }
}
