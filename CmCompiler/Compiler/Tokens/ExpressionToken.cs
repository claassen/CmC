﻿using System;
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
    [TokenExpression("EXPRESSION", "BOOLEAN_EXPRESSION")]
    public class ExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens[0] is ICodeEmitter)
            {
                ((ICodeEmitter)Tokens[0]).Emit(context);
            }   
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[0]).GetExpressionType(context);
        }
    }
}
