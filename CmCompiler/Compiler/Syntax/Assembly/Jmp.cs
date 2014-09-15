using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("JMP", "JMP_OP (REGISTER|IMMEDIATE|IMM_LABEL)")]
    public class Jmp : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Jmp() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens[1] is RegisterToken)
            {
                if (((JmpOp)Tokens[0]).Condition == JmpOp.JmpCond.NONE)
                {
                    context.EmitInstruction(new IRJumpRegister() { Address = ((RegisterToken)Tokens[1]).Name });
                }
                else
                {
                    throw new Exception("Only unconditional jump supports register operand");
                }
            }
            else
            {
                ImmediateValue address = null;

                if (Tokens[1] is ImmediateValueToken)
                {
                    address = ((ImmediateValueToken)Tokens[1]).GetValue(context);
                }
                else if (Tokens[1] is LabelAddressToken)
                {
                    address = ((LabelAddressToken)Tokens[1]).GetValue(context);
                }

                switch(((JmpOp)Tokens[0]).Condition)
                {
                    case JmpOp.JmpCond.EQ:
                        context.EmitInstruction(new IRJumpEQ() { Address = address });
                        break;
                    case JmpOp.JmpCond.GE:
                        context.EmitInstruction(new IRJumpGE() { Address = address });
                        break;
                    case JmpOp.JmpCond.GT:
                        context.EmitInstruction(new IRJumpGT() { Address = address });
                        break;
                    case JmpOp.JmpCond.LE:
                        context.EmitInstruction(new IRJumpLE() { Address = address });
                        break;
                    case JmpOp.JmpCond.LT:
                        context.EmitInstruction(new IRJumpLT() { Address = address });
                        break;
                    case JmpOp.JmpCond.NE:
                        context.EmitInstruction(new IRJumpNE() { Address = address });
                        break;
                    case JmpOp.JmpCond.NONE:
                        context.EmitInstruction(new IRJumpImmediate() { Address = address });
                        break;
                }
            }
        }
    }

    [TokenExpression("REGEX:JMP_OP", "'j(eq|ge|gt|mp|le|lt|ne)'")] 
    public class JmpOp : ILanguageTerminalToken
    {
        public enum JmpCond
        {
            EQ,
            GE,
            GT,
            LE,
            LT,
            NE,
            NONE
        }

        public JmpCond Condition;

        public override ILanguageToken Create(string expressionValue)
        {
            JmpCond condition;

            if (expressionValue.EndsWith("eq"))
            {
                condition = JmpCond.EQ;
            }
            else if (expressionValue.EndsWith("ge"))
            {
                condition = JmpCond.GE;
            }
            else if (expressionValue.EndsWith("gt"))
            {
                condition = JmpCond.GT;
            }
            else if (expressionValue.EndsWith("mp"))
            {
                condition = JmpCond.NONE;
            }
            else if (expressionValue.EndsWith("le"))
            {
                condition = JmpCond.LE;
            }
            else if (expressionValue.EndsWith("lt"))
            {
                condition = JmpCond.LT;
            }
            else if (expressionValue.EndsWith("ne"))
            {
                condition = JmpCond.NE;
            }
            else
            {
                throw new Exception("Unknown jump condition");
            }

            return new JmpOp() { Condition = condition };
        }
    }
}
