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
            Console.WriteLine("\n;Function definition");

            string functionName = ((VariableToken)Tokens[0]).Name;

            context.AddFunctionSymbol(functionName);

            context.NewScope(true);

            for (int i = 1; i < Tokens.Count - 1; i++)
            {
                string parameterName = ((VariableToken)Tokens[i]).Name;

                context.AddFunctionArgSymbol(parameterName);
            }

            ((ICodeEmitter)Tokens.Last()).Emit(context);

            context.EndScope(true);
        }
    }
}
