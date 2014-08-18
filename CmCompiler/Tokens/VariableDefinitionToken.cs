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
            string variableName = ((VariableToken)Tokens[1]).Name;

            context.AddVariableSymbol(variableName);

            if (Tokens.Count > 2)
            {
                ((ICodeEmitter)Tokens[3]).Emit(context);

                int? address = context.GetVariableAddress(variableName);

                if (address == null)
                {
                    throw new Exception("Undefined variable: " + variableName);
                }

                context.Emit("pop eax");
                context.Emit("store eax -> " + address);
            }
        }
    }
}
