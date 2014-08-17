using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("VARIABLE_DEFINITION", "'var' VARIABLE ('=' EXPRESSION)?")]
    public class VariableDefinitionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new VariableDefinitionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine("make space on stack, add variable to symbol table");

            if (Tokens.Count > 2)
            {
                ((ICodeEmitter)Tokens[3]).Emit(context);
                Console.WriteLine("pop eax");
                Console.WriteLine("store [addressOf(variable)] <- eax");
            }
        }
    }
}
