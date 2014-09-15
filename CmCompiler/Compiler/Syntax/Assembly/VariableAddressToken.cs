using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("IMM_VAR", "'[' IDENTIFIER ']'")]
    public class VariableAddressToken : ILanguageNonTerminalToken
    {
        public string VariableName;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new VariableAddressToken() { VariableName = ((IdentifierToken)tokens[1]).Name };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            var variable = context.GetVariable(VariableName);

            return variable.Address;
        }
    }
}
