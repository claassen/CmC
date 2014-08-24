using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("TYPE", "('int' | 'bool' | IDENTIFIER)")]
    public class TypeToken : IUserLanguageNonTerminalToken
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new TypeToken() { Tokens = tokens };
        }

        public string GetTypeName()
        {
            if (Tokens[0] is VariableToken)
            {
                return ((VariableToken)Tokens[0]).Name;
            }
            else
            {
                return ((DefaultLanguageTerminalToken)Tokens[0]).Value;
            }
        }
    }
}
