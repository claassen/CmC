using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("FUNCTION_BODY", "'{' (STATEMENT)* '}'")]
    public class FunctionBodyToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionBodyToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            for (int i = 1; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }
        }
    }
}
