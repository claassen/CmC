using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Exceptions;
using CmC.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [TokenExpression("FUNCTION_CALL", "IDENTIFIER '(' (EXPRESSION (',' EXPRESSION)*)? ')'")]
    public class FunctionCallToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionCallToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Function call");

            string functionName = ((IdentifierToken)Tokens[0]).Name;

            var function = context.GetFunction(functionName);

            //Push base pointer on stack
            context.EmitInstruction(new Op() { Name = "push", R1 = "bp" });

            int argumentCount = 0;

            if (Tokens.Count > 1)
            {
                //Push arguments on stack in reverse order
                for (int i = Tokens.Count - 1; i > 0; i--)
                {
                    var argType = ((IHasType)Tokens[i]).GetExpressionType(context);
                    var paramType = function.ParameterTypes[function.ParameterTypes.Count - (Tokens.Count - 1)];

                    ExpressionType.CheckTypesMatch(paramType, argType);

                    ((ICodeEmitter)Tokens[i]).Emit(context);
                    argumentCount++;
                }
            }

            if (argumentCount != function.ParameterTypes.Count)
            {
                throw new ArgumentCountMismatchException(functionName, function.ParameterTypes.Count, argumentCount);
            }

            int currentInstrAddress = context.GetCurrentInstructionAddress();
            int functionReturnAddress = currentInstrAddress + 4;

            //Push return address on stack and jump to function
            context.EmitInstruction(new Op() { Name = "push", Imm = new AbsoluteAddressValue(functionReturnAddress) });

            //Set base pointer to be the top of current function's stack which will be the bottom
            //of the called function's stack
            context.EmitInstruction(new Op { Name = "mov", R1 = "sp", R2 = "bp" });
            
            //Jump to function location
            context.EmitInstruction(new Op() { Name = "jmp", Imm = function.Address });

            //Resume here
            for (int i = 0; i < argumentCount; i++)
            {
                //Reclaim stack space from arguments pushed before the call
                context.EmitInstruction(new Op() { Name = "pop" });
            }

            //Reset base pointer
            context.EmitInstruction(new Op() { Name = "pop", R1 = "bp" });

            context.EmitInstruction(new Op() { Name = "push", R1 = "eax" });
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            string functionName = ((IdentifierToken)Tokens[0]).Name;

            var function = context.GetFunction(functionName);

            return function.ReturnType;
        }

        public void EmitAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of function call");

            //Emitting the address of a function call -> address of where return value (temporary) is stored on the stack?
        }
    }
}
