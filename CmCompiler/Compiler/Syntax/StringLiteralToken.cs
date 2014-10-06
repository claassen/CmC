using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("REGEX:STRING", "'\".*\"'")] 
    public class StringLiteralToken : ILanguageTerminalToken, ICodeEmitter, IHasAddress, IHasType
    {
        public string Value;

        public override ILanguageToken Create(string expressionValue)
        {
            return new StringLiteralToken() { Value = expressionValue.Replace("\\n", "\n") };
        }

        public void Emit(CompilationContext context)
        {
            context.AddStringConstant(Value);

            EmitAddress(context);
        }

        public void EmitAddress(CompilationContext context)
        {
            context.AddStringConstant(Value);

            int label = context.GetStringConstantLabelAddress(Value);

            context.EmitInstruction(new IRPushImmediate() { Value = new LabelAddressValue(label) });
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return new ExpressionType()
            {
                Type = new TypeDef() { Name = "byte", Size = 1 },
                IsArray = true,
                ArrayLength = Value.Length - 1, //-2 to remove quotes, +1 for trailing \0
                IndirectionLevel = 1
            };
        }
    }
}
