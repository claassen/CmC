using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.Common.Interface;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("IMMEDIATE", "(NUMBER | IMM_VAR | IMM_LABEL)")]
    public class ImmediateValueToken : ILanguageNonTerminalToken, IHasValue
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ImmediateValueToken() { Tokens = tokens };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            return ((IHasValue)Tokens[0]).GetValue(context);
        }
    }
}
