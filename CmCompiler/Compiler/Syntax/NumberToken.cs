﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("REGEX:NUMBER", "'[0-9]+'")]
    public class NumberToken : ILanguageTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public int Value;

        public override ILanguageToken Create(string expressionValue)
        {
            return new NumberToken() { Value = Int32.Parse(expressionValue) };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new IRPushImmediate() { Value = new ImmediateValue(Value) });
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return new ExpressionType() { Type = context.GetTypeDef("int") };
        }

        public void EmitAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of number literal");
        }
    }
}
