﻿using System;
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
    [TokenExpression("TYPE_SPECIFIER", "(TYPE | FUNC_TYPE_SPECIFIER) ('*')* ('[' (NUMBER)? ']')?")]
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
                    BaseType = context.GetTypeDef(((TypeToken)Tokens[0]).GetTypeName())
                };
            }
            else if (Tokens[0] is FuncTypeSpecifierToken)
            {
                type = ((FuncTypeSpecifierToken)Tokens[0]).GetExpressionType(context);
            }

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

            return type;
        }
    }
}
