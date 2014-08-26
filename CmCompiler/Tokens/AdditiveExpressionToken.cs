using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Exceptions;
using CmC.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ADDITIVE_EXPRESSION", "MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*")]
    public class AdditiveExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
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
                //Type.CheckTypesMatch(t1, t2);
                t1 = t2;

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

                switch (op)
                {
                    case "+":
                        context.EmitInstruction(new Op() { Name = "add", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                    case "-":
                        context.EmitInstruction(new Op() { Name = "sub", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                }
                
                context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
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
