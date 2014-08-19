using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ADDITIVE_EXPRESSION", "MULTIPLICATIVE_EXPRESSION (('+'|'-') MULTIPLICATIVE_EXPRESSION)*")]
    public class AdditiveExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AdditiveExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Additive expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i+1]).Emit(context);

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

                switch (op)
                {
                    case "+":
                        context.EmitInstruction(new Op() { Name = "add", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                    case "-":
                        context.EmitInstruction(new Op() { Name = "sub", R1 = "eax", R2 = "ebx", R3 = "ecx" });
                        break;
                }
                
                context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
            }
        }
    }
}
