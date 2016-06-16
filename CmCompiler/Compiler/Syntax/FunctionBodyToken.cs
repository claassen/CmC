﻿using System;
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
    [TokenExpression("FUNCTION_BODY", "'{' (STATEMENT | RETURN_STATEMENT)* '}'")]
    public class FunctionBodyToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionBodyToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    ((ICodeEmitter)token).Emit(context);
                }
            }
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            int sum = 0;

            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    sum += ((ICodeEmitter)token).GetSizeOfAllLocalVariables(context);
                }
            }

            return sum;
        }
    }
}
