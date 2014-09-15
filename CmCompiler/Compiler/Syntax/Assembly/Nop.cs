using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("NOP", "'nop'")]
    public class Nop : ILanguageTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue)
        {
            return new Nop();
        }

        public void Emit(Context.CompilationContext context)
        {
            context.EmitInstruction(new IRNoop());
        }
    }
}
