using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("EQUALITY_EXPRESSION", "ADDITIVE_EXPRESSION (('=='|'!='|'<'|'>'|'<='|'>=') ADDITIVE_EXPRESSION)*")]
    public class EqualityExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new EqualityExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            ((ICodeEmitter)Tokens[0]).Emit(context);
            ((ICodeEmitter)Tokens[2]).Emit(context);
            Console.WriteLine("pop eax");
            Console.WriteLine("pop ebx");
            Console.WriteLine("cmp eax, ebx");
        }
    }
}
