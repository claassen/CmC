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
    [TokenExpression("MOV", "'mov' REGISTER '<-' (IMMEDIATE | REGISTER)")]
    public class Mov : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Mov() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            string destRegister = ((RegisterToken)Tokens[1]).Name;

            if (Tokens[3] is ImmediateValueToken)
            {
                ImmediateValue value = ((ImmediateValueToken)Tokens[3]).GetValue(context);

                context.EmitInstruction(new IRMoveImmediate() { To = destRegister, Value = value });
            }
            else
            {
                string srcRegister = ((RegisterToken)Tokens[3]).Name;

                context.EmitInstruction(new IRMoveRegister() { To = destRegister, From = srcRegister });
            }
        }
    }
}
