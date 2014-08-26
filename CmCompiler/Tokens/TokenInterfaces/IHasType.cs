using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;

namespace CmC.Tokens.TokenInterfaces
{
    public interface IHasType
    {
        ExpressionType GetExpressionType(CompilationContext context);
    }
}
