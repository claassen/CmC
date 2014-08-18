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
            Console.WriteLine(";Return statement");
            
            ((ICodeEmitter)Tokens[1]).Emit(context);

            context.Emit("pop eax");

            for (int i = 0; i < context.GetFunctionLocalVarCount(); i++)
            {
                context.Emit("pop");
            }

            //Pop return address of stack and jump
            context.Emit("pop ebx");
            context.Emit("jmp ebx");
        }
    }
}
