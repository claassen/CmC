using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.Common.Interface;
using CmC.Compiler.Syntax.Common;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("IMM_VAR", "'&' IDENTIFIER")]
    public class VariableAddressToken : ILanguageNonTerminalToken, IHasValue
    {
        public string VariableName;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new VariableAddressToken() { VariableName = ((IdentifierToken)tokens[1]).Name };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            var variable = context.GetVariable(VariableName);

            //TODO: how to handle stack address values?

            if (variable.Address is StackAddressValue)
            {
                //throw new Exception("Stack address values not supported for assembly variable references");
                return new ImmediateValue(variable.Address.Value);
            }

            return variable.Address;
        }
    }
}
