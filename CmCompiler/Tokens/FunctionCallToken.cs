using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("FUNCTION_CALL", "VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'")]
    public class FunctionCallToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionCallToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("push arguments on stack");
            Console.WriteLine("call [addressOf(function)]");
        }
    }
}
