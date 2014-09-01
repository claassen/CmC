using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Syntax
{
    [TokenExpression("ELSEIF", "'else' 'if' '(' EXPRESSION ')' '{' (STATEMENT)* '}'")]
    public class ElseIfToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ElseIfToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Else if");

            //context.EmitLabel("ELSE" + context.ElseIfLabelCount++);
            context.EmitLabel(context.ConditionalElseBranchLabels.Pop());

            int elseBranchLabel = context.CreateNewLabel();
            context.ConditionalElseBranchLabels.Push(elseBranchLabel);

            ((ICodeEmitter)Tokens[2]).Emit(context);

            //context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
            context.EmitInstruction(new IRPop() { To = "eax" });

            //context.EmitInstruction(new Op() { Name = "cmp", Imm = new ImmediateValue(0) });
            context.EmitInstruction(new IRCompareImmediate() { Left = "eax", Right = new ImmediateValue(0) });

            //context.EmitInstruction(new Op() { Name = "je", Imm = new LabelAddressValue(elseBranchLabel) });
            context.EmitInstruction(new IRJumpEQ() { Address = new LabelAddressValue(elseBranchLabel) });

            context.EmitComment(";Then");

            context.NewScope(false);

            for (int i = 4; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            context.EndScope(false);
        }
    }
}
