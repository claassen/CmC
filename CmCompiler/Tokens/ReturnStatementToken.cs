using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("RETURN_STATEMENT", "'return' EXPRESSION")] 
    public class ReturnStatementToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ReturnStatementToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Return statement");

            context.ReportReturnStatement();

            ((ICodeEmitter)Tokens[1]).Emit(context);

            //Retrurn value from function goes in eax
            context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

            //Reclaim local variables from stack space
            for (int i = 0; i < context.GetFunctionLocalVarCount(); i++)
            {
                context.EmitInstruction(new Op() { Name = "pop" });
            }

            //Pop return address of stack and jump
            context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

            //Reset callee's base pointer
            context.EmitInstruction(new Op() { Name = "pop", R1 = "bp" });

            //Resume execution from call site
            context.EmitInstruction(new Op() { Name = "jmp", R1 = "ebx" });
        }
    }
}
