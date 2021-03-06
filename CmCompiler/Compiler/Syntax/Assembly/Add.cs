﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("ADD", "'add' REGISTER ',' REGISTER '->' REGISTER")]
    public class Add : ILanguageNonTerminalToken, ICodeEmitter
    {
        public string Left;
        public string Right;
        public string Destination;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Add()
            {
                Left = ((RegisterToken)tokens[1]).Name,
                Right = ((RegisterToken)tokens[2]).Name,
                Destination = ((RegisterToken)tokens[4]).Name
            };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new IRAdd() { Left = Left, Right = Right, To = Destination });
        }


        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
