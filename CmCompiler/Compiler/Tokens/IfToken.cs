using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Tokens
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

            //context.ElseIfLabelCount = 0;
            int elseBranchLabel = context.CreateNewLabel();
            context.ConditionalElseBranchLabels.Push(elseBranchLabel);

            ((ICodeEmitter)Tokens[1]).Emit(context);

            //context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
            context.EmitInstruction(new IRPop() { To = "eax" });

            //context.EmitInstruction(new Op() { Name = "cmp", R1 = "eax", Imm = new ImmediateValue(0) });
            context.EmitInstruction(new IRCompareImmediate() { Left = "eax", Right = new ImmediateValue(0) });
            
            //context.EmitInstruction(new Op() { Name = "je", Imm = new LabelAddressValue(elseBranchLabel) });
            context.EmitInstruction(new IRJumpEQ() { Address = new LabelAddressValue(elseBranchLabel) });

            context.EmitComment(";Then");

            context.NewScope(false);
            context.StartPossiblyNonExecutedBlock();

            for (int i = 3; i < Tokens.Count - 1; i++)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            context.EndPossiblyNonExecutedBlock();
            context.EndScope(false);
        }
    }
}
