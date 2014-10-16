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

            var leftSideType = ((IHasType)Tokens[0]).GetExpressionType(context);
            var rightSideType = ((IHasType)Tokens[2]).GetExpressionType(context);

            if (leftSideType.IsArray)
            {
                throw new TypeMismatchException(new ExpressionType() { IsArray = true, Type = leftSideType.Type, ArrayLength = leftSideType.ArrayLength }, rightSideType);
            }

            if (leftSideType.GetSize() == 0)
            {
                throw new VoidAssignmentException("to");
            }
            else if (rightSideType.GetSize() == 0)
            {
                throw new VoidAssignmentException("from");
            }

            ExpressionType.CheckTypesMatch(leftSideType, rightSideType);

            //right hand side value -> stack
            ((ICodeEmitter)Tokens[2]).Emit(context);

            if (rightSideType.GetSize() > 4)
            {
                //[sp] -> [destination]
                //Dest address -> eax
                ((IHasAddress)Tokens[0]).EmitAddress(context);
                context.EmitInstruction(new IRPop() { To = "eax" });

                //sp -= size of value
                context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(rightSideType.GetSize()) });
                context.EmitInstruction(new IRSub() { Left = "sp", Right = "ebx", To = "sp" });

                context.EmitInstruction(new IRMemCopy() { From = "sp", To = "eax", Length = new ImmediateValue(rightSideType.GetSize()) });
            }
            else
            {
                //store ebx -> [destination]
                ((IHasAddress)Tokens[0]).EmitAddress(context);
                context.EmitInstruction(new IRPop() { To = "eax" });

                //Store assign value in ebx
                context.EmitInstruction(new IRPop() { To = "ebx" });
                context.EmitInstruction(new IRStoreRegister() { From = "ebx", To = "eax", OperandSize = rightSideType.GetSize() }); //MB!
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[2]).GetExpressionType(context);
        }

        public void EmitAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of assignment expression");
        }
    }
}
