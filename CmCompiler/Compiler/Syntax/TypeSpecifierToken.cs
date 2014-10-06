using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.Common;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("TYPE_SPECIFIER", "(TYPE ('*')* ('[' (NUMBER)? ']')? | FUNC_TYPE_SPECIFIER)")]
    public class TypeSpecifierToken : ILanguageNonTerminalToken, IHasType
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new TypeSpecifierToken() { Tokens = tokens };
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            ExpressionType type = null;

            if (Tokens[0] is TypeToken)
            {
                type = new ExpressionType()
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
                                type.ArrayLength = ((NumberToken)Tokens[i + 1]).GetValue(context).Value;
                            }
                            else
                            {
                                type.ArrayLength = -1;
                            }

                            break;
                        }
                    }
                }
            }
            else if (Tokens[0] is FuncTypeSpecifierToken)
            {
                type = ((FuncTypeSpecifierToken)Tokens[0]).GetExpressionType(context);
            }

            return type;
        }
    }
}
