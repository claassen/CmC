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
            Console.WriteLine(";Function call");

            for (int i = Tokens.Count - 1; i > 0; i--)
            {
                ((ICodeEmitter)Tokens[i]).Emit(context);
            }

            string functionName = ((VariableToken)Tokens[0]).Name;

            int? funcAddress = context.GetFunctionAddress(functionName);

            if (funcAddress == null)
            {
                throw new Exception("Undefined symbol: " + functionName);
            }

            int currentInstrAddress = context.GetCurrentInstructionAddress();
            int functionReturnAddress = currentInstrAddress + 3;

            //Push return address on stack and jump to function
            context.Emit("push $" + functionReturnAddress, "Push return address on stack");
            context.Emit("jmp " + funcAddress);
            context.Emit("push eax");
        }
    }
}
