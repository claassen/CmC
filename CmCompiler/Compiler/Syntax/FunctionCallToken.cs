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

            var type = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (!type.IsFunction)
            {
                throw new Exception("Can't call expression type: " + type + " as a function");
            }

            ExpressionType returnType = type.ReturnType;
            List<ExpressionType> parameterTypes = type.ArgumentTypes;
            
            if (returnType.GetSize() > 4)
            {
                //Make space for return value in caller stack
                //context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = new ImmediateValue(function.ReturnType.GetSize()) });
                //context.EmitInstruction(new IRAdd() { Left = "sp", Right = "eax", To = "sp" });
                throw new LargeReturnValuesNotSupportedException();
            }

            //Push base pointer on stack
            context.EmitInstruction(new IRPushRegister() { From = "bp" });

            int argumentCount = Tokens.Count - 1;
            int argumentsSize = 0;

            if (argumentCount != parameterTypes.Count)
            {
                throw new ArgumentCountMismatchException(Tokens[0].ToString(), parameterTypes.Count, argumentCount);
            }

            if (Tokens.Count > 1)
            {
                //Push arguments on stack in reverse order
                for (int i = Tokens.Count - 1; i > 0; i--)
                {
                    var argType = ((IHasType)Tokens[i]).GetExpressionType(context);
                    var paramType = parameterTypes[parameterTypes.Count - 1 - (Tokens.Count - 1 - i)];

                    ExpressionType.CheckTypesMatch(paramType, argType);

                    //Push argument value on stack
                    ((ICodeEmitter)Tokens[i]).Emit(context);
                    
                    argumentCount++;
                    argumentsSize += argType.GetSize();
                }
            }

            var returnLabel = new LabelAddressValue(context.CreateNewLabel());

            //Expression value -> eax
            ((ICodeEmitter)Tokens[0]).Emit(context);
            context.EmitInstruction(new IRPop() { To = "eax" });

            //Set base pointer to be the top of current function's stack which will be the bottom
            //of the called function's stack
            context.EmitInstruction(new IRMoveRegister() { From = "sp", To = "bp" });

            //Push return address
            context.EmitInstruction(new IRPushImmediate() { Value = returnLabel });
            
            context.EmitInstruction(new IRJumpRegister() { Address = "eax" });

            //Resume here, reclaim space from arguments pushed on stack
            context.EmitLabel(returnLabel.Value);
            context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(argumentsSize) });
            context.EmitInstruction(new IRSub() { To = "sp", Left = "sp", Right = "ebx" });

            //Reset base pointer
            context.EmitInstruction(new IRPop() { To = "bp" });

            if (returnType.GetSize() > 4)
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
            var funcType = ((IHasType)Tokens[0]).GetExpressionType(context);

            return funcType.ReturnType;
        }

        public void EmitAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of function call");

            //Emitting the address of a function call -> address of where return value (temporary) is stored on the stack?
        }
    }
}
