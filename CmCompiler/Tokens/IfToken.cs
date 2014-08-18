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
            Console.WriteLine("\n;If");

            context.ElseIfCount = 0;

            ((ICodeEmitter)Tokens[1]).Emit(context);

            context.Emit("pop eax");
            context.Emit("cmp eax, $0");
            context.Emit("je ELSE" + context.ElseIfCount);

            Console.WriteLine(";Then");

            context.NewScope(false);

            for (int i = 3; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            context.EndScope(false);
        }
    }
}
