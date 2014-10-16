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

namespace CmC.Compiler.Syntax
{
    [TokenExpression("IF", "'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'")]
    public class IfToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new IfToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";If");

            int elseBranchLabel = context.CreateNewLabel();
            context.ConditionalElseBranchLabels.Push(elseBranchLabel);

            ((ICodeEmitter)Tokens[1]).Emit(context);

            context.EmitInstruction(new IRPop() { To = "eax" });
            context.EmitInstruction(new IRCompareImmediate() { Left = "eax", Right = new ImmediateValue(0) });
            context.EmitInstruction(new IRJumpEQ() { Address = new LabelAddressValue(elseBranchLabel) });

            context.EmitComment(";Then");

            context.NewScope();
            context.StartPossiblyNonExecutedBlock();

            for (int i = 3; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            //Skip the rest of the conditional blocks
            context.EmitInstruction(new IRJumpImmediate() { Address = new LabelAddressValue(context.ConditionalEndLabels.Peek()) });

            context.EndPossiblyNonExecutedBlock();
            context.EndScope(false);
        }
    }
}
