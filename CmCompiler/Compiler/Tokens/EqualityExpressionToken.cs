using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Tokens
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

                //context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new IRPop() { To = "eax" });

                //context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });
                context.EmitInstruction(new IRPop() { To = "ebx" });

                //context.EmitInstruction(new Op() { Name = "cmp", R1 = "eax", R2 = "ebx" });
                context.EmitInstruction(new IRCompareRegister() { Left = "eax", Right = "ebx" });

                //int currentInstrAddress = context.GetCurrentInstructionAddress();
                //int trueJmpLocation = currentInstrAddress + 4;

                var trueLabel = new LabelAddressValue(context.CreateNewLabel());

                switch (op)
                {
                    case "==":
                        //context.EmitInstruction(new Op() { Name = "je", Imm = trueLabel });
                        context.EmitInstruction(new IRJumpEQ() { Address = trueLabel });
                        break;
                    case "!=":
                        //context.EmitInstruction(new Op() { Name = "jne", Imm = trueLabel });
                        context.EmitInstruction(new IRJumpNE() { Address = trueLabel });
                        break;
                    case ">":
                        //context.EmitInstruction(new Op() { Name = "jl", Imm = trueLabel });
                        context.EmitInstruction(new IRJumpLT() { Address = trueLabel });
                        break;
                    case "<":
                        //context.EmitInstruction(new Op() { Name = "jg", Imm = trueLabel });
                        context.EmitInstruction(new IRJumpGT() { Address = trueLabel });
                        break;
                    case ">=":
                        //context.EmitInstruction(new Op() { Name = "jge", Imm = trueLabel });
                        context.EmitInstruction(new IRJumpGE() { Address = trueLabel });
                        break;
                    case "<=":
                        //context.EmitInstruction(new Op() { Name = "jle", Imm = trueLabel });
                        context.EmitInstruction(new IRJumpLE() { Address = trueLabel });
                        break;
                }

                var skipTrueLabel = new LabelAddressValue(context.CreateNewLabel());

                //context.EmitInstruction(new Op() { Name = "push", Imm = new ImmediateValue(0) }, "FALSE");
                context.EmitInstruction(new IRPushImmediate() { Value = new ImmediateValue(0) });
                //context.EmitInstruction(new Op() { Name = "jmp", Imm = skipTrueLabel });
                context.EmitInstruction(new IRJumpImmediate() { Address = skipTrueLabel });
                context.EmitLabel(trueLabel.Number);
                //context.EmitInstruction(new Op() { Name = "push", Imm = new ImmediateValue(1) }, "TRUE");
                context.EmitInstruction(new IRPushImmediate() { Value = new ImmediateValue(1) });
                context.EmitLabel(skipTrueLabel.Number);
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
