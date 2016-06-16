using System;
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

            var t1ExpressionType = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (Tokens.Count > 1)
            {
                TypeChecking.CheckExpressionTypeIsNumeric(t1ExpressionType);
            }

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                var t2ExpressionType = ((IHasType)Tokens[i + 1]).GetExpressionType(context);
                
                TypeChecking.CheckExpressionTypeIsNumeric(t2ExpressionType);
                TypeChecking.CheckExpressionTypesMatch(t1ExpressionType, t2ExpressionType);
                
                t1ExpressionType = t2ExpressionType;

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.EmitInstruction(new IRPop() { To = "ebx" });
                context.EmitInstruction(new IRPop() { To = "eax" });

                switch (op)
                {
                    case "*":
                        context.EmitInstruction(new IRMult() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                    case "/":
                        context.EmitInstruction(new IRDiv() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                }
                
                context.EmitInstruction(new IRPushRegister() { From = "ecx" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[0]).GetExpressionType(context);
        }

        public void PushAddress(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                throw new Exception("Can't take address of multiplicative expression");
            }

            ((IHasAddress)Tokens[0]).PushAddress(context);
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
