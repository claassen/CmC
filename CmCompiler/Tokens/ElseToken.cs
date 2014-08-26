using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ELSE", "'else' '{' (STATEMENT)* '}'")]
    public class ElseToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ElseToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Else");

            context.EmitLabel("ELSE" + context.ElseIfLabelCount);

            context.EmitComment(";Then");

            context.NewScope(false);
            context.StartPossiblyNonExecutedBlock();

            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    ((ICodeEmitter)token).Emit(context);
                }
            }

            context.EndPossiblyNonExecutedBlock();
            context.EndScope(false);
        }
    }
}
