using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Syntax
{
    [TokenExpression("MULTIPLICATIVE_EXPRESSION", "CAST_EXPRESSION (('*'|'/') CAST_EXPRESSION)*")]
    public class MultiplicativeExpression : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
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

                //context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new IRPop() { To = "eax" });

                //context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });
                context.EmitInstruction(new IRPop() { To = "ebx" });

                switch (op)
                {
                    case "*":
                        //context.EmitInstruction(new Op() { Name = "mult", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        context.EmitInstruction(new IRMult() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                    case "/":
                        //context.EmitInstruction(new Op() { Name = "div", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        context.EmitInstruction(new IRDiv() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                }
                
                //context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
                context.EmitInstruction(new IRPushRegister() { From = "ecx" });
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
