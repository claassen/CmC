using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("BOOLEAN_EXPRESSION", "EQUALITY_EXPRESSION (('&&'|'||') EQUALITY_EXPRESSION)*")]
    public class BooleanExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new BooleanExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("evaluate boolean expression, store result on stack");
        }
    }
}
