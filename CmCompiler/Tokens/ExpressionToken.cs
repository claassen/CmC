using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("EXPRESSION", "BOOLEAN_EXPRESSION")]
    public class ExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens[0] is ICodeEmitter)
            {
                ((ICodeEmitter)Tokens[0]).Emit(context);
            }   
        }

        public Type GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[0]).GetExpressionType(context);
        }
    }
}
