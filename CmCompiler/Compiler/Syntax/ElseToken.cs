using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("ELSE", "'else' '{' (STATEMENT)* '}'")]
    public class ElseToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ElseToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Else");

            context.EmitLabel(context.ConditionalElseBranchLabels.Pop());

            //See ConditionalToken for why this extra label is created
            context.ConditionalElseBranchLabels.Push(context.CreateNewLabel());

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
