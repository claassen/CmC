using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.IR;
using CmC.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Syntax
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

            if (function.ReturnType.GetSize() > 4)
            {
                //Make space for return value in caller stack
                //context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = new ImmediateValue(function.ReturnType.GetSize()) });
                //context.EmitInstruction(new IRAdd() { Left = "sp", Right = "eax", To = "sp" });
                throw new LargeReturnValuesNotSupportedException();
            }

            //Push base pointer on stack
            context.EmitInstruction(new IRPushRegister() { From = "bp" });

            int argumentCount = 0;
            int argumentsSize = 0;

            if (Tokens.Count > 1)
            {
                //Push arguments on stack in reverse order
                for (int i = Tokens.Count - 1; i > 0; i--)
                {
                    var argType = ((IHasType)Tokens[i]).GetExpressionType(context);
                    var paramType = function.ParameterTypes[function.ParameterTypes.Count - (Tokens.Count - 1)];

                    ExpressionType.CheckTypesMatch(paramType, argType);

                    //Push argument value on stack
                    ((ICodeEmitter)Tokens[i]).Emit(context);
                    
                    argumentCount++;
                    argumentsSize += argType.GetSize();
                }
            }

            if (argumentCount != function.ParameterTypes.Count)
            {
                throw new ArgumentCountMismatchException(functionName, function.ParameterTypes.Count, argumentCount);
            }

            var returnLabel = new LabelAddressValue(context.CreateNewLabel());

            //Push return address on stack and jump to function
            context.EmitInstruction(new IRPushImmediate() { Value = returnLabel });

            //Set base pointer to be the top of current function's stack which will be the bottom
            //of the called function's stack
            context.EmitInstruction(new IRMoveRegister() { From = "sp", To = "bp" });
            
            //Jump to function location
            context.EmitInstruction(new IRJumpImmediate() { Address = function.Address });

            //Resume here
            for (int i = 0; i < argumentsSize / 4; i++)
            {
                //Reclaim stack space from arguments pushed before the call
                context.EmitInstruction(new IRPop() { To = "nil" });
            }

            //Reset base pointer
            context.EmitInstruction(new IRPop() { To = "bp" });

            if (function.ReturnType.GetSize() > 4)
            {
                //Return value is already on stack
                throw new LargeReturnValuesNotSupportedException();
            }
            else
            {
                //Return value in eax, put on stack
                context.EmitInstruction(new IRPushRegister() { From = "eax" });
            }
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
