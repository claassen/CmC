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
    [TokenExpression("STORE", "'store' REGISTER '->' (REGISTER | IMMEDIATE | REG_PLUS_IMM)")]
    public class Store : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Store() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            string value = ((RegisterToken)Tokens[1]).Name;

            if (Tokens[3] is RegisterToken)
            {
                context.EmitInstruction(new IRStoreRegister() { From = value, To = ((RegisterToken)Tokens[3]).Name });
            }
            else if (Tokens[3] is ImmediateValueToken)
            {
                context.EmitInstruction(new IRStoreImmediate() { From = value, To = ((ImmediateValueToken)Tokens[3]).GetValue(context) });
            }
            else if (Tokens[3] is RegisterPlusImmediateToken)
            {
                var regPlusImmediateToken = (RegisterPlusImmediateToken)Tokens[3];

                context.EmitInstruction(new IRStoreRegisterPlusImmediate() { From = value, To = regPlusImmediateToken.Register, Offset = regPlusImmediateToken.GetImmediateValue(context) });
            }
        }
    }
}
