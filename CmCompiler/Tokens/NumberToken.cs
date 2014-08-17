using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("REGEX:NUMBER", "'[0-9]+'")]
    public class NumberToken : IUserLanguageTerminalToken, ICodeEmitter
    {
        public int Value;

        public override IUserLanguageToken Create(string expressionValue)
        {
            return new NumberToken() { Value = Int32.Parse(expressionValue) };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("push " + Value);
        }
    }
}
