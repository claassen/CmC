using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("IMMEDIATE", "(IMM_DEC | IMM_VAR)")]
    public class ImmediateValueToken : ILanguageNonTerminalToken
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ImmediateValueToken() { Tokens = tokens };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            if (Tokens[0] is DecimalNumberToken)
            {
                return ((DecimalNumberToken)Tokens[0]).GetValue(context);
            }
            else if (Tokens[0] is VariableAddressToken)
            {
                return ((VariableAddressToken)Tokens[0]).GetValue(context);
            }
            else if (Tokens[0] is LabelAddressToken)
            {
                return ((LabelAddressToken)Tokens[0]).GetValue(context);
            }
            else
            {
                throw new Exception("Unexpected immediate value token type");
            }
        }
    }
}
