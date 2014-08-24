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
    public class ReturnStatementToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType
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

            //Caller saves registers

            //Return value from function goes in eax
            context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

            //Reclaim local variables from stack space
            for (int i = 0; i < context.GetFunctionLocalVarCount(); i++)
            {
                context.EmitInstruction(new Op() { Name = "pop" });
            }

            //Pop return address off stack and jump
            context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });
            context.EmitInstruction(new Op() { Name = "jmp", R1 = "ebx" });
        }

        public Type GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[1]).GetExpressionType(context);
        }
    }
}
