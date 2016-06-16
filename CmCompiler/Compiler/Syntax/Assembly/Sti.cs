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
    [TokenExpression("STI", "'sti'")]
    public class Sti : ILanguageTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue)
        {
            return new Sti();
        }

        public void Emit(Context.CompilationContext context)
        {
            context.EmitInstruction(new IRArchitectureSpecificAsm("sti"));
        }


        public int GetSizeOfAllLocalVariables(Context.CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
