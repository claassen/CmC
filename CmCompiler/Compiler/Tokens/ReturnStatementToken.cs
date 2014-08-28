﻿using System;
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
    [TokenExpression("RETURN_STATEMENT", "'return' EXPRESSION")]
    public class ReturnStatementToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ReturnStatementToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Return statement");

            context.ReportReturnStatement();

            ((ICodeEmitter)Tokens[1]).Emit(context);

            //Caller saves registers

            //Return value from function goes in eax
            //context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
            context.EmitInstruction(new IRPop() { To = "eax" });

            //Reclaim local variables from stack space
            for (int i = 0; i < context.GetFunctionLocalVarCount(); i++)
            {
                //context.EmitInstruction(new Op() { Name = "pop" });
                context.EmitInstruction(new IRPop() { To = "nil" });
            }

            //Pop return address off stack and jump
            //context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });
            context.EmitInstruction(new IRPop() { To = "ebx" });

            //context.EmitInstruction(new Op() { Name = "jmp", R1 = "ebx" });
            context.EmitInstruction(new IRJumpRegister() { Address = "ebx" });
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[1]).GetExpressionType(context);
        }
    }
}
