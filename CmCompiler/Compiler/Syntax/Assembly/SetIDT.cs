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
    [TokenExpression("SETIDT", "'setidt' IMMEDIATE")]
    public class SetIDT : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new SetIDT() { Tokens = tokens };
        }

        public void Emit(Context.CompilationContext context)
        {
            context.EmitInstruction(new IRSetIDT() { Address = ((ImmediateValueToken)Tokens[1]).GetValue(context) });
        }
    }
}
