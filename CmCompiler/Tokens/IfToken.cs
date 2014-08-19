using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("IF", "'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'")]
    public class IfToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new IfToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";If");

            context.ElseIfLabelCount = 0;

            ((ICodeEmitter)Tokens[1]).Emit(context);

            context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
            context.EmitInstruction(new Op() { Name = "cmp", R1 = "eax", Imm = new ImmediateValue(0) });
            context.EmitInstruction(new Op() { Name = "je", Imm = new LabelAddressValue("ELSE" + context.ElseIfLabelCount.ToString()) });

            context.EmitComment(";Then");

            context.NewScope(false);

            for (int i = 3; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            context.EndScope(false);
        }
    }
}
