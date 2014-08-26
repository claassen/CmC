using System;
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
            context.EmitInstruction(new Op() { Name = "push", Imm = new ImmediateValue(Value) });
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
