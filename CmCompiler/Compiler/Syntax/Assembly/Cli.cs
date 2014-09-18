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
    [TokenExpression("CLI", "'cli'")]
    public class Cli : ILanguageTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue)
        {
            return new Cli();
        }

        public void Emit(Context.CompilationContext context)
        {
            context.EmitInstruction(new IRArchitectureSpecificAsm("cli"));
        }
    }
}
