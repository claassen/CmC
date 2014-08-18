using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("MULTIPLICATIVE_EXPRESSION", "PRIMARY (('*'|'/') PRIMARY)*")]
    public class MultiplicativeExpression : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new MultiplicativeExpression() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            ((ICodeEmitter)Tokens[0]).Emit(context);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.Emit("pop -> eax");
                context.Emit("pop -> ebx");
                context.Emit("mult eax, ebx -> ecx");
                context.Emit("push ecx");
            }
        }
    }
}
