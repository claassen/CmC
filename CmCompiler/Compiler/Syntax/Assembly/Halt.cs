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
    [TokenExpression("HALT", "'halt'")]
    public class Halt : ILanguageTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue)
        {
            return new Halt();
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new IRHalt());
        }
    }
}
