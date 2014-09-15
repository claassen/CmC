using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("REGEX:REGISTER", "'[a-z]+'")]
    public class RegisterToken : ILanguageTerminalToken
    {
        public string Name;

        public override ILanguageToken Create(string expressionValue)
        {
            return new RegisterToken() { Name = expressionValue };
        }
    }
}
