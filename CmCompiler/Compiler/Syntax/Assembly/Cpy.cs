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
    [TokenExpression("CPY", "'cpy' REGISTER '->' REGISTER ',' IMMEDIATE")]
    public class Cpy : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Cpy() { Tokens = tokens };
        }

        public void Emit(Context.CompilationContext context)
        {
            string from = ((RegisterToken)Tokens[1]).Name;
            string to = ((RegisterToken)Tokens[3]).Name;
            var length = ((ImmediateValueToken)Tokens[5]).GetValue(context);

            context.EmitInstruction(new IRMemCopy() { From = from, To = to, Length = length });
        }
    }
}
