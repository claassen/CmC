using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Tokens
{
    [TokenExpression("TYPE_SPECIFIER", "TYPE ('*')*")]
    public class TypeSpecifierToken : ILanguageNonTerminalToken, IHasType
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new TypeSpecifierToken() { Tokens = tokens };
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            var type = new ExpressionType() 
            { 
                Type = context.GetTypeDef(((TypeToken)Tokens[0]).GetTypeName())
            };

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
