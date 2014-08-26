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
    [TokenExpression("EQUALITY_EXPRESSION", "ADDITIVE_EXPRESSION (('=='|'!='|'<'|'>'|'<='|'>=') ADDITIVE_EXPRESSION)?")]
    public class EqualityExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new EqualityExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Equality expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var t1 = ((IHasType)Tokens[0]).GetExpressionType(context);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                var t2 = ((IHasType)Tokens[i + 1]).GetExpressionType(context);

                ExpressionType.CheckTypesMatch(t1, t2);
                t1 = t2;

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

                context.EmitInstruction(new Op() { Name = "cmp", R1 = "eax", R2 = "ebx" });

                int currentInstrAddress = context.GetCurrentInstructionAddress();
                int trueJmpLocation = currentInstrAddress + 4;

                switch (op)
                {
                    case "==":
                        context.EmitInstruction(new Op() { Name = "je", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                    case "!=":
                        context.EmitInstruction(new Op() { Name = "jne", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                    case ">":
                        context.EmitInstruction(new Op() { Name = "jl", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                    case "<":
                        context.EmitInstruction(new Op() { Name = "jg", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                    case ">=":
                        context.EmitInstruction(new Op() { Name = "jge", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                    case "<=":
                        context.EmitInstruction(new Op() { Name = "jle", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                }

                context.EmitInstruction(new Op() { Name = "FALSE: push", Imm = new ImmediateValue(0) });
                context.EmitInstruction(new Op() { Name = "jmp", Imm = new AbsoluteAddressValue(trueJmpLocation + 1) });
                context.EmitInstruction(new Op() { Name = "TRUE: push", Imm = new ImmediateValue(1) });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                return new ExpressionType() { Type = context.GetTypeDef("bool") };
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
                throw new Exception("Can't take address of equality expression");
            }

            ((IHasAddress)Tokens[0]).EmitAddress(context);
        }
    }
}
