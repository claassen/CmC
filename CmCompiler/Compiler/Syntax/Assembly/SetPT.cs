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
    [TokenExpression("SETPT", "'setpt' IMMEDIATE")]
    public class SetPT : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new SetPT() { Tokens = tokens };
        }

        public void Emit(Context.CompilationContext context)
        {
            context.EmitInstruction(new IRSetPT() { Address = ((ImmediateValueToken)Tokens[1]).GetValue(context) });
        }


        public int GetSizeOfAllLocalVariables(Context.CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
