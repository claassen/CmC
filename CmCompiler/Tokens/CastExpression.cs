using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("CAST_EXPRESSION", "('(' TYPE_SPECIFIER ')')? UNARY_EXPRESSION")]
    public class CastExpression : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new CastExpression() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            //TODO: need to change emitted value if it's cast?
            ((ICodeEmitter)Tokens.Last()).Emit(context);
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[0]).GetExpressionType(context);
        }

        public void EmitAddress(CompilationContext context)
        {
            ((IHasAddress)Tokens.Last()).EmitAddress(context);
        }
    }
}
