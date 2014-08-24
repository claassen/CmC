using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("TYPE_SPECIFIER", "TYPE ('*')*")]
    public class TypeSpecifierToken : IUserLanguageNonTerminalToken, IHasType
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new TypeSpecifierToken() { Tokens = tokens };
        }

        public Type GetExpressionType(CompilationContext context)
        {
            var type = new Type() { Name = ((TypeToken)Tokens[0]).GetTypeName() };

            for (int i = 1; i < Tokens.Count; i++)
            {
                if (Tokens[i] is DefaultLanguageTerminalToken)
                {
                    if (((DefaultLanguageTerminalToken)Tokens[i]).Value == "*")
                    {
                        type.IndirectionLevel++;
                    }
                }
            }

            return type;
        }
    }
}
