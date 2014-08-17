using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("IF", "'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'")]
    public class IfToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new IfToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("evaluate if");
        }
    }
}
