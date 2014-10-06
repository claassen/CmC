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
    [TokenExpression("RETURN_STATEMENT", "'return' EXPRESSION ';'")]
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

            if (((IHasType)Tokens[1]).GetExpressionType(context).GetSize() > 4)
            {
                //Return value gets placed space allocated in caller's stack
                throw new Exception("Large return values not supported");
            }
            else
            {
                //Return value from function goes in eax
                context.EmitInstruction(new IRPop() { To = "eax" });
            }

            int localVarsSize = context.GetFunctionLocalVarSize();

            //Reclaim local variables from stack space
            //context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(localVarsSize) });
            //context.EmitInstruction(new IRSub() { To = "sp", Left = "sp", Right = "ebx" });
            context.EmitInstruction(new IRMoveRegister() { To = "sp", From = "bp" });

            if (context.IsEntryPointFunction)
            {
                //At this point the return value of the function is still in eax and we simple halt execution
                //as the exe has run to completion
                context.EmitInstruction(new IRHalt());
            }
            else
            {
                //Pop return address off stack and jump
                context.EmitInstruction(new IRRet());
                
                //context.EmitInstruction(new IRPop() { To = "ebx" });

                //context.EmitInstruction(new IRJumpRegister() { Address = "ebx" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[1]).GetExpressionType(context);
        }
    }
}
