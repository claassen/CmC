using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("PRIMARY", "('(' BOOLEAN_EXPRESSION ')' | NUMBER | FUNCTION_CALL | VARIABLE | MINUS PRIMARY)")]
    public class PrimaryToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new PrimaryToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("evaluate primary");
        }
    }
}
