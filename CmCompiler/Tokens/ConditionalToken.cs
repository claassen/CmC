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
    [TokenExpression("CONDITIONAL", "IF (ELSEIF)* (ELSE)?")]
    public class ConditionalToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ConditionalToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    ((ICodeEmitter)token).Emit(context);
                }
            }
        }
    }
}
