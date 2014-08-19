﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ASSIGNMENT", "VARIABLE '=' EXPRESSION")]
    public class AssignmentToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AssignmentToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Assignment");

            ((ICodeEmitter)Tokens[2]).Emit(context);

            context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

            var address = context.GetVariableAddress(((VariableToken)Tokens[0]).Name);

            context.EmitInstruction(new Op() { Name = "store", Imm = address });
        }
    }
}