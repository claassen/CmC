using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.Common.Interface;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Common
{
    [TokenExpression("NUMBER", "(NUM_DEC|NUM_HEX)")]
    public class NumberToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress, IHasValue
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new NumberToken() { Tokens = tokens };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            return ((IHasValue)Tokens[0]).GetValue(context);
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new IRPushImmediate() { Value = GetValue(context) });
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
