﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Tokens.TokenInterfaces;
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
