using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ASSIGNMENT", "VARIABLE '=' EXPRESSION")]
    public class AssignmentToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AssignmentToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            ((ICodeEmitter)Tokens[2]).Emit(context);
            Console.WriteLine("pop eax");
            Console.WriteLine("store memory[addressOf(" + ((VariableToken)Tokens[0]).Name + ")] <- eax");
        }
    }
}
