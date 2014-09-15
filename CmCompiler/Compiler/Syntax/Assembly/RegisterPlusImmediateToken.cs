using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("REG_PLUS_IMM", "'[' REGISTER '+' IMMEDIATE ']'")]
    public class RegisterPlusImmediateToken : ILanguageNonTerminalToken
    {
        public string Register;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new RegisterPlusImmediateToken() { Tokens = tokens, Register = ((RegisterToken)tokens[1]).Name };
        }

        public ImmediateValue GetImmediateValue(CompilationContext context)
        {
            return ((ImmediateValueToken)Tokens[3]).GetValue(context);
        }
    }
}
