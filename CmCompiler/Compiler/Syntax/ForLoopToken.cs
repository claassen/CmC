﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("FOR_LOOP", "'for' '(' (VARIABLE_DEFINITION|ASSIGNMENT|FUNCTION_CALL) ';' BOOLEAN_EXPRESSION ';' (ASSIGNMENT|FUNCTION_CALL) ')' '{' (STATEMENT)* '}'")]
    public class ForLoopToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public ICodeEmitter Initializer;
        public ICodeEmitter Condition;
        public ICodeEmitter Update;
        public List<ILanguageToken> Body;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ForLoopToken() 
            { 
                Tokens = tokens,
                Initializer = (ICodeEmitter)tokens[1],
                Condition = (ICodeEmitter)tokens[3],
                Update = (ICodeEmitter)tokens[5],
                Body = tokens.Skip(7).ToList()
            };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";For");

            context.NewScope(false);
            context.StartPossiblyNonExecutedBlock();

            int testLabel = context.CreateNewLabel();
            int doneLabel = context.CreateNewLabel();

            Initializer.Emit(context);
            context.EmitLabel(testLabel);
            Condition.Emit(context);
            context.EmitInstruction(new IRPop() { To = "eax" });
            context.EmitInstruction(new IRCompareImmediate() { Left = "eax", Right = new ImmediateValue(0) });
            context.EmitInstruction(new IRJumpEQ() { Address = new LabelAddressValue(doneLabel) });

            foreach (var token in Body)
            {
                if (token is ICodeEmitter)
                {
                    ((ICodeEmitter)token).Emit(context);
                }
            }

            Update.Emit(context);
            context.EmitInstruction(new IRJumpImmediate() { Address = new LabelAddressValue(testLabel) });
            context.EmitLabel(doneLabel);

            context.EndPossiblyNonExecutedBlock();
            context.EndScope(false);
        }
    }
}