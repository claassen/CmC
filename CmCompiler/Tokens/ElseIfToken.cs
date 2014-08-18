using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ELSEIF", "'else' 'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'")]
    public class ElseIfToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ElseIfToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine(";Else if");

            Console.WriteLine("ELSE" + context.ElseIfCount++ + ":");

            ((ICodeEmitter)Tokens[2]).Emit(context);

            context.Emit("pop eax");
            context.Emit("cmp eax, $0");
            context.Emit("je ELSE" + context.ElseIfCount);

            Console.WriteLine(";Then");

            context.NewScope(false);

            for (int i = 4; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            context.EndScope(false);
        }
    }
}
