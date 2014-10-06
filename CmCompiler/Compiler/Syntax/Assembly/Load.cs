using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.Common;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("LOAD", "'load' (SIZE_MOD)? REGISTER '<-' (REGISTER | IMMEDIATE | REG_PLUS_IMM)")]
    public class Load : ILanguageNonTerminalToken, ICodeEmitter
    {
        public int NumBytes;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            int numBytes = 4;

            if (tokens[1] is DataSizeModifierToken)
            {
                numBytes = ((DataSizeModifierToken)tokens[1]).Size;
                tokens.RemoveAt(1);
            }

            return new Load() { Tokens = tokens , NumBytes = numBytes };
        }

        public void Emit(CompilationContext context)
        {
            string destination = ((RegisterToken)Tokens[1]).Name;

            if (Tokens[3] is RegisterToken)
            {
                context.EmitInstruction(new IRLoadRegister() { To = destination, From = ((RegisterToken)Tokens[3]).Name, OperandSize = NumBytes });
            }
            else if (Tokens[3] is ImmediateValueToken)
            {
                var address = ((ImmediateValueToken)Tokens[3]).GetValue(context);

                if (address is StackAddressValue)
                {
                    context.EmitInstruction(new IRLoadRegisterPlusImmediate() { To = destination, From = "bp", Offset = new ImmediateValue(address.Value), OperandSize = NumBytes });
                }
                else
                {
                    context.EmitInstruction(new IRLoadImmediate() { To = destination, Address = address, OperandSize = NumBytes });
                }
            }
            else if (Tokens[3] is RegisterPlusImmediateToken)
            {
                var regPlusImmediateToken = (RegisterPlusImmediateToken)Tokens[3];

                context.EmitInstruction(new IRLoadRegisterPlusImmediate() { To = destination, From = regPlusImmediateToken.Register, Offset = regPlusImmediateToken.GetImmediateValue(context), OperandSize = NumBytes });
            }
        }
    }
}
