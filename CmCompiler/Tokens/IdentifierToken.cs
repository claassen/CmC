using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("REGEX:IDENTIFIER", "'[a-zA-Z]+'")]
    public class IdentifierToken : IUserLanguageTerminalToken
    {
        public string Name;

        public override IUserLanguageToken Create(string expressionValue)
        {
            return new IdentifierToken() { Name = expressionValue };
        }
    }
}
