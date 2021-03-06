﻿using System;
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
    [TokenExpression("CONDITIONAL", "IF (ELSEIF)* (ELSE)?")]
    public class ConditionalToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new ConditionalToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            int endLabel = context.CreateNewLabel();
            context.ConditionalEndLabels.Push(endLabel);

            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    ((ICodeEmitter)token).Emit(context);
                }
            }

            //When an If or Else if pushes the next branch label on the ConditionalElseBranch label stack
            //it can't know if there is going to be another Else if or Else which will pop it. We fix this 
            //by having Else always push a new label as well so we always are left with an unused label on the 
            //ConditionalElseBranch label stack at the end of a ConditionalToken
            context.EmitLabel(context.ConditionalElseBranchLabels.Pop());

            context.EmitLabel(context.ConditionalEndLabels.Pop());
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            int sum = 0;

            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    sum += ((ICodeEmitter)token).GetSizeOfAllLocalVariables(context);
                }
            }

            return sum;
        }
    }
}
