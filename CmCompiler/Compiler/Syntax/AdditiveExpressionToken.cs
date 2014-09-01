using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("ADDITIVE_EXPRESSION", "MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*")]
    public class AdditiveExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AdditiveExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Additive expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            ExpressionType t1 = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (Tokens.Count > 1)
            {
                ExpressionType.CheckTypeIsNumeric(t1);
            }

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i+1]).Emit(context);

                ExpressionType t2 = ((IHasType)Tokens[i + 1]).GetExpressionType(context);
                ExpressionType.CheckTypeIsNumeric(t2);
                
                t1 = t2;

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.EmitInstruction(new IRPop() { To = "eax" });
                context.EmitInstruction(new IRPop() { To = "ebx" });

                switch (op)
                {
                    case "+":
                        context.EmitInstruction(new IRAdd() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                    case "-":
                        context.EmitInstruction(new IRSub() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                }
                
                context.EmitInstruction(new IRPushRegister() { From = "ecx" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            //TODO: deal with int + double = double etc.
            return ((IHasType)Tokens[0]).GetExpressionType(context);
        }

        public void EmitAddress(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                Emit(context);
            }
            else
            {
                ((IHasAddress)Tokens[0]).EmitAddress(context);
            }
        }
    }
}
