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
    [TokenExpression("PUSH", "'push' (REGISTER|IMMEDIATE)")]
    public class Push : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Push() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens[1] is RegisterToken)
            {
                context.EmitInstruction(new IRPushRegister() { From = ((RegisterToken)Tokens[1]).Name });
            }
            else if (Tokens[1] is ImmediateValueToken)
            {
                context.EmitInstruction(new IRPushImmediate() { Value = ((ImmediateValueToken)Tokens[1]).GetValue(context) });
            }
        }
    }
}
