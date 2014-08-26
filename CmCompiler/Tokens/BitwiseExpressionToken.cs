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
    [UserLanguageToken("BITWISE_EXPRESSION", "EQUALITY_EXPRESSION (('&'|'|'|'^') EQUALITY_EXPRESSION)*")]
    public class BitwiseExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new BitwiseExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Bitwise expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var type = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (Tokens.Count > 1)
            {
                ExpressionType.CheckTypeIsNumeric(type);
            }

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                type = ((IHasType)Tokens[i + 1]).GetExpressionType(context);

                ExpressionType.CheckTypeIsNumeric(type);

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

                switch (op)
                {
                    case "&":
                        context.EmitInstruction(new Op() { Name = "and", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                    case "|":
                        context.EmitInstruction(new Op() { Name = "or", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                    case "^":
                        context.EmitInstruction(new Op() { Name = "xor", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                }

                context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                return new ExpressionType() { Type = context.GetTypeDef("int") };
            }
            else
            {
                return ((IHasType)Tokens[0]).GetExpressionType(context);
            }
        }

        public void EmitAddress(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                throw new Exception("Can't take address of bitwise expression");
            }

            ((IHasAddress)Tokens[0]).EmitAddress(context);
        }
    }
}
