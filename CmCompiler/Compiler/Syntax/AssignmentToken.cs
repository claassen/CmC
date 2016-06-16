using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("ASSIGNMENT", "UNARY_EXPRESSION '=' EXPRESSION")]
    public class AssignmentToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AssignmentToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Assignment");

            var leftSideExpressionType = ((IHasType)Tokens[0]).GetExpressionType(context);
            var rightSideExpressionType = ((IHasType)Tokens[2]).GetExpressionType(context);

            if (leftSideExpressionType.IsArray)
            {
                throw new TypeMismatchException(new ExpressionType() { IsArray = true, BaseType = leftSideExpressionType.BaseType, ArrayLength = leftSideExpressionType.ArrayLength }, rightSideExpressionType);
            }

            if (leftSideExpressionType.GetSize() == 0)
            {
                throw new VoidAssignmentException("to");
            }
            else if (rightSideExpressionType.GetSize() == 0)
            {
                throw new VoidAssignmentException("from");
            }

            TypeChecking.CheckExpressionTypesMatch(leftSideExpressionType, rightSideExpressionType);

            //Special case for assignment of string literal to byte array:
            //  Don't emit string constant normally (which would add it as a string constant in the data section),
            //  instead copy the string to the memory occupied by tge byte array itself
            if (rightSideExpressionType.BaseType is StringLiteralTypeDef && leftSideExpressionType.IsArray)
            {
                string stringLiteral = ((StringLiteralTypeDef)rightSideExpressionType.BaseType).Value;

                if (rightSideExpressionType.ArrayLength > leftSideExpressionType.ArrayLength)
                {
                    throw new Exception("The string '" + stringLiteral + "' is too large to be assigned by value to the left hand expression.");
                }

                //Put start address to copy string value to into eax
                ((IHasAddress)Tokens[0]).PushAddress(context);
                context.EmitInstruction(new IRPop() { To = "eax" });

                for (int i = 0; i < leftSideExpressionType.ArrayLength; i++)
                {
                    byte charValue = i < stringLiteral.Length ? (byte)stringLiteral[i] : (byte)0;

                    //Put char value into ebx
                    context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(charValue), OperandSize = 1 });
                    //Store char value into address [eax + char offset]
                    context.EmitInstruction(new IRStoreRegisterPlusImmediate() { To = "eax", Offset = new ImmediateValue(i), From = "ebx", OperandSize = 1 });
                }
            }
            else
            {
                //right hand side value -> stack
                ((ICodeEmitter)Tokens[2]).Emit(context);

                if (rightSideExpressionType.GetSize() > 4)
                {
                    //[sp] -> [destination]
                    //Dest address -> eax
                    ((IHasAddress)Tokens[0]).PushAddress(context);
                    context.EmitInstruction(new IRPop() { To = "eax" });

                    //sp -= size of value
                    context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(rightSideExpressionType.GetSize()) });
                    context.EmitInstruction(new IRSub() { Left = "sp", Right = "ebx", To = "sp" });

                    context.EmitInstruction(new IRMemCopy() { From = "sp", To = "eax", Length = new ImmediateValue(rightSideExpressionType.GetSize()) });
                }
                else
                {
                    //store ebx -> [destination]
                    ((IHasAddress)Tokens[0]).PushAddress(context);
                    context.EmitInstruction(new IRPop() { To = "eax" });

                    //Store assign value in ebx
                    context.EmitInstruction(new IRPop() { To = "ebx" });
                    context.EmitInstruction(new IRStoreRegister() { From = "ebx", To = "eax", OperandSize = rightSideExpressionType.GetSize() }); //MB!
                }
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[2]).GetExpressionType(context);
        }

        public void PushAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of assignment expression");
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
