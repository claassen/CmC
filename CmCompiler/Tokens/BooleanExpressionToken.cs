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
    [UserLanguageToken("BOOLEAN_EXPRESSION", "BITWISE_EXPRESSION (('&&'|'||') BITWISE_EXPRESSION)*")]
    public class BooleanExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new BooleanExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Boolean expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var type = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (Tokens.Count > 1)
            {
                ExpressionType.CheckTypeIsBoolean(type); 
            }

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                type = ((IHasType)Tokens[i + 1]).GetExpressionType(context);

                ExpressionType.CheckTypeIsBoolean(type);

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

                int currentInstrAddress = context.GetCurrentInstructionAddress();
                int trueJmpLocation = currentInstrAddress + 6;

                switch (op)
                {
                    case "&&":
                        context.EmitInstruction(new Op() { Name = "and", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        context.EmitInstruction(new Op() { Name = "cmp", R1 = "ecx", Imm = new ImmediateValue(0) });
                        context.EmitInstruction(new Op() { Name = "jne", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                    case "||":
                        context.EmitInstruction(new Op() { Name = "or", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        context.EmitInstruction(new Op() { Name = "cmp", R1 = "ecx", Imm = new ImmediateValue(0) });
                        context.EmitInstruction(new Op() { Name = "jne", Imm = new AbsoluteAddressValue(trueJmpLocation) });
                        break;
                }

                context.EmitInstruction(new Op() { Name = "push", Imm = new ImmediateValue(0) }, "FALSE");
                context.EmitInstruction(new Op() { Name = "jmp", Imm = new AbsoluteAddressValue(trueJmpLocation + 1) });
                context.EmitInstruction(new Op() { Name = "push", Imm = new ImmediateValue(1) });
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
                throw new Exception("Can't take address of boolean expression");
            }

            ((IHasAddress)Tokens[0]).EmitAddress(context);
        }
    }
}
