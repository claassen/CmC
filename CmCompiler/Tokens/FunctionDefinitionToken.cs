using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("FUNCTION_DEFINITION", "VARIABLE '(' (VARIABLE (',' VARIABLE)*)? ')' FUNCTION_BODY")]
    public class FunctionDefinitionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionDefinitionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("add function to symbol table, create new scope and add arguments to symbol table");
            ((ICodeEmitter)Tokens.Last()).Emit(context);
        }
    }
}
