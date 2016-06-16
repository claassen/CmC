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
            return new StringLiteralToken() { Value = expressionValue.Replace("\\n", "\n").Trim('"') };
        }

        public void Emit(CompilationContext context)
        {
            context.AddStringConstant(Value);

            PushAddress(context);
        }

        public void PushAddress(CompilationContext context)
        {
            context.AddStringConstant(Value);

            int label = context.GetStringConstantLabelAddress(Value);

            context.EmitInstruction(new IRPushImmediate() { Value = new LabelAddressValue(label) });
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return new ExpressionType()
            {
                BaseType = new StringLiteralTypeDef() 
                { 
                    Name = "byte", 
                    Size = 1,
                    Value = Value
                },
                IsArray = true,
                ArrayLength = Value.Length + 1, //+1 for trailing \0
                IndirectionLevel = 1,
            };
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
