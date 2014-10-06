using System;
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
    [TokenExpression("INT", "'int' IMMEDIATE")]
    public class Int : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Int() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new IRInt() { InterruptNumber = ((ImmediateValueToken)Tokens[1]).GetValue(context).Value });
        }
    }
}
