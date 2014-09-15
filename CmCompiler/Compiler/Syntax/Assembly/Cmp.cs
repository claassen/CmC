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
    [TokenExpression("CMP", "'cmp' REGISTER ',' (REGISTER | IMMEDIATE)")]
    public class Cmp : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Cmp() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            string left = ((RegisterToken)Tokens[0]).Name;

            if (Tokens[2] is RegisterToken)
            {
                context.EmitInstruction(new IRCompareRegister() { Left = left, Right = ((RegisterToken)Tokens[2]).Name });
            }
            else
            {
                context.EmitInstruction(new IRCompareImmediate() { Left = left, Right = ((ImmediateValueToken)Tokens[2]).GetValue(context) });
            }
        }
    }
}
