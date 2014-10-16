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
using CmC.Compiler.Exceptions;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("RETURN_STATEMENT", "'return' (EXPRESSION)? ';'")]
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

            var returnType = new ExpressionType() { Type = new TypeDef() { Name = "void", Size = 0 } };

            if (Tokens.Count > 1)
            {
                returnType = ((IHasType)Tokens[1]).GetExpressionType(context);

                ((ICodeEmitter)Tokens[1]).Emit(context);

                //Caller saves registers

                if (returnType.GetSize() > 4)
                {
                    //Return value gets placed space allocated in caller's stack
                    throw new Exception("Large return values not supported");
                }
                else
                {
                    //Return value from function goes in eax
                    context.EmitInstruction(new IRPop() { To = "eax" });
                }
            }

            int localVarsSize = context.GetFunctionLocalVarSize();

            //Reclaim local variables from stack space
            context.EmitInstruction(new IRMoveRegister() { To = "sp", From = "bp" });

            ExpressionType.CheckTypesMatch(context.GetCurrentFunctionReturnType(), returnType);
            
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
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[1]).GetExpressionType(context);
        }
    }
}
