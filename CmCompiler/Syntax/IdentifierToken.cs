using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Syntax
{
    [TokenExpression("REGEX:IDENTIFIER", "'[a-zA-Z_][a-zA-Z_0-9]*'")]
    public class IdentifierToken : ILanguageTerminalToken
    {
        public string Name;

        public override ILanguageToken Create(string expressionValue)
        {
            return new IdentifierToken() { Name = expressionValue };
        }
    }
}
