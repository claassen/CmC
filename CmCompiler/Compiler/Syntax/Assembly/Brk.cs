using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("BRK", "'brk'")]
    public class Brk : ILanguageTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue)
        {
            return new Brk();
        }

        public void Emit(Context.CompilationContext context)
        {
            context.EmitInstruction(new IRBreak());
        }


        public int GetSizeOfAllLocalVariables(Context.CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
