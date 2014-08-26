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
    [UserLanguageToken("MULTIPLICATIVE_EXPRESSION", "CAST_EXPRESSION (('*'|'/') CAST_EXPRESSION)*")]
    public class MultiplicativeExpression : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new MultiplicativeExpression() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Multiplicative expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var t1 = ((IHasType)Tokens[0]).GetExpressionType(context);

            ExpressionType.CheckTypeIsNumeric(t1);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                var t2 = ((IHasType)Tokens[i + 1]).GetExpressionType(context);
                ExpressionType.CheckTypeIsNumeric(t2);
                ExpressionType.CheckTypesMatch(t1, t2);
                t1 = t2;

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

                switch (op)
                {
                    case "*":
                        context.EmitInstruction(new Op() { Name = "mult", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                    case "/":
                        context.EmitInstruction(new Op() { Name = "div", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                }
                
                context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[0]).GetExpressionType(context);
        }

        public void EmitAddress(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                throw new Exception("Can't take address of multiplicative expression");
            }

            ((IHasAddress)Tokens[0]).EmitAddress(context);
        }
    }
}
