using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("TYPE_SPECIFIER", "TYPE ('*')* ('[' (NUMBER)? ']')?")]
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
                    else if (((DefaultLanguageTerminalToken)Tokens[i]).Value == "[")
                    {
                        type.IsArray = true;
                        type.IndirectionLevel++;

                        if (Tokens[i + 1] is NumberToken)
                        {
                            type.ArrayLength = ((NumberToken)Tokens[i + 1]).Value;
                        }
                        else
                        {
                            type.ArrayLength = -1;
                        }

                        break;
                    }
                }
            }

            return type;
        }
    }
}
