using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("FUNCTION_CALL", "VARIABLE '(' (EXPRESSION (',' EXPRESSION)*)? ')'")]
    public class FunctionCallToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionCallToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Function call");

            //Push base pointer on stack
            context.EmitInstruction(new Op() { Name = "push", R1 = "bp" });

            if (Tokens.Count > 1)
            {
                //Push arguments on stack in reverse order
                for (int i = Tokens.Count - 1; i > 0; i--)
                {
                    ((ICodeEmitter)Tokens[i]).Emit(context);
                }
            }

            string functionName = ((VariableToken)Tokens[0]).Name;

            var funcAddress = context.GetFunctionAddress(functionName);

            int currentInstrAddress = context.GetCurrentInstructionAddress();
            int functionReturnAddress = currentInstrAddress + 4;

            //Push return address on stack and jump to function
            context.EmitInstruction(new Op() { Name = "push", Imm = new AbsoluteAddressValue(functionReturnAddress) });

            //Set base pointer to be the top of current function's stack which will be the bottom
            //of the called function's stack
            context.EmitInstruction(new Op { Name = "mov", R1 = "sp", R2 = "bp" });
            
            //Jump to function location
            context.EmitInstruction(new Op() { Name = "jmp", Imm = funcAddress });

            //Resume here
            context.EmitInstruction(new Op() { Name = "push", R1 = "eax" });
        }
    }
}
