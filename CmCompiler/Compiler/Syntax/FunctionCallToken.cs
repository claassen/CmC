using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.Common;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("FUNCTION_CALL", "PRIMARY_EXPRESSION '(' (EXPRESSION (',' EXPRESSION)*)? ')'")]
    public class FunctionCallToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionCallToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Function call");

            var expressionType = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (!(expressionType.BaseType is FunctionTypeDef))
            {
                throw new Exception("Can't call expression type: " + expressionType + " as a function");
            }

            FunctionTypeDef functionType = (FunctionTypeDef)expressionType.BaseType;
            ExpressionType returnType = functionType.ReturnType;
            //List<ExpressionType> parameterTypes = functionType.ArgumentTypes;
            
            if (returnType.GetSize() > 4)
            {
                //Make space for return value in caller stack
                //context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = new ImmediateValue(function.ReturnType.GetSize()) });
                //context.EmitInstruction(new IRAdd() { Left = "sp", Right = "eax", To = "sp" });
                throw new LargeReturnValuesNotSupportedException();
            }

            //Push base pointer on stack
            context.EmitInstruction(new IRPushRegister() { From = "bp" });

            //Save registers (TODO: Not actually needed until we have smarter allocation that uses registers instead of stack)
            //context.EmitInstruction(new IRPushRegister() { From = "eax" });
            //context.EmitInstruction(new IRPushRegister() { From = "ebx" });
            //context.EmitInstruction(new IRPushRegister() { From = "ecx" });
            //context.EmitInstruction(new IRPushRegister() { From = "edx" });

            int argumentCount = Tokens.Count - 1;
            int argumentsSize = 0;

            if (argumentCount != functionType.ArgumentTypes.Count)
            {
                throw new ArgumentCountMismatchException(Tokens[0].ToString(), functionType.ArgumentTypes.Count, argumentCount);
            }

            if (Tokens.Count > 1)
            {
                //Push arguments on stack in reverse order
                for (int i = Tokens.Count - 1; i > 0; i--)
                {
                    var argExpressionType = ((IHasType)Tokens[i]).GetExpressionType(context);
                    var paramExpressionType = functionType.ArgumentTypes[functionType.ArgumentTypes.Count - 1 - (Tokens.Count - 1 - i)];

                    TypeChecking.CheckExpressionTypesMatch(paramExpressionType, argExpressionType);

                    //Push argument value on stack
                    ((ICodeEmitter)Tokens[i]).Emit(context);
                    
                    argumentCount++;
                    argumentsSize += argExpressionType.GetSize();
                }
            }

            var returnLabel = new LabelAddressValue(context.CreateNewLabel());

            //Address of function -> eax
            ((ICodeEmitter)Tokens[0]).Emit(context);
            context.EmitInstruction(new IRPop() { To = "eax" });

            //Set base pointer to be the top of current function's stack which will be the bottom
            //of the called function's stack
            context.EmitInstruction(new IRMoveRegister() { From = "sp", To = "bp" });

            //Push return address
            context.EmitInstruction(new IRPushImmediate() { Value = returnLabel });
            
            //Jump to function
            context.EmitInstruction(new IRJumpRegister() { Address = "eax" });

            //Resume here, reclaim space from arguments pushed on stack
            context.EmitLabel(returnLabel.Value);
            context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(argumentsSize) });
            context.EmitInstruction(new IRSub() { To = "sp", Left = "sp", Right = "ebx" });

            //Restore registers (TODO: Not actually needed until we have smarter allocation that uses registers instead of stack)
            //context.EmitInstruction(new IRPop() { To = "edx" });
            //context.EmitInstruction(new IRPop() { To = "ecx" });
            //context.EmitInstruction(new IRPop() { To = "ebx" });
            //context.EmitInstruction(new IRPop() { To = "eax" });

            //Reset base pointer
            context.EmitInstruction(new IRPop() { To = "bp" });

            if (returnType.GetSize() > 4)
            {
                //Return value is already on stack
                throw new LargeReturnValuesNotSupportedException();
            }
            else if(returnType.GetSize() > 0)
            {
                //Return value in eax, put on stack
                context.EmitInstruction(new IRPushRegister() { From = "eax" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            var functionCallExpressionType = ((IHasType)Tokens[0]).GetExpressionType(context);

            return ((FunctionTypeDef)functionCallExpressionType.BaseType).ReturnType;
        }

        public void PushAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of function call");

            //Emitting the address of a function call -> address of where return value (temporary) is stored on the stack?
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
