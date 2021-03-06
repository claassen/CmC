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
    [TokenExpression("POP", "'pop' REGISTER")]
    public class Pop : ILanguageNonTerminalToken, ICodeEmitter
    {
        public string Register;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Pop() { Register = ((RegisterToken)tokens[1]).Name };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new IRPop() { To = Register });
        }


        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
