using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.Common.Interface;
using CmC.Compiler.Syntax.Common;
using ParserGen.Parser.Tokens;
using CmC.Compiler.Exceptions;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("IMM_VAR", "'&' IDENTIFIER")]
    public class VariableAddressToken : ILanguageNonTerminalToken, IHasValue
    {
        public string Identifier;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new VariableAddressToken() { Identifier = ((IdentifierToken)tokens[1]).Name };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            if (context.IsVariableDefined(Identifier))
            {
                var variable = context.GetVariable(Identifier);

                //Note: variable address must be used as [bp + &variable] if the variable is a stack address to get
                //the correct address
                if (variable.Address is StackAddressValue)
                {
                    return new ImmediateValue(variable.Address.Value);
                }

                return variable.Address;
            }
            else if (context.IsFunctionDefined(Identifier))
            {
                var function = context.GetFunction(Identifier);

                return function.Address;
            }

            throw new UndefinedVariableException(Identifier);
        }
    }
}
