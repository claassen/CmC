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
    [TokenExpression("POSTFIX_EXPRESSION", "(PRIMARY_EXPRESSION (('.'|'->') IDENTIFIER | '[' EXPRESSION ']')? | FUNCTION_CALL)")]
    public class PostfixExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public bool IsFieldAccess;        //.
        public bool IsPointerFieldAccess; //->
        public bool IsArrayIndex;         //[EXPRESSION]

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            if (tokens.Count > 1)
            {
                string op = ((DefaultLanguageTerminalToken)tokens[1]).Value;

                switch (op)
                {
                    case ".":
                        return new PostfixExpressionToken() { Tokens = tokens, IsFieldAccess = true };
                    case "->":
                        return new PostfixExpressionToken() { Tokens = tokens, IsPointerFieldAccess = true };
                    case "[":
                        return new PostfixExpressionToken() { Tokens = tokens, IsArrayIndex = true };
                    default:
                        throw new Exception("This shouldn't happen");
                }
            }
            else
            {
                return new PostfixExpressionToken() { Tokens = tokens };
            }
        }

        public void Emit(CompilationContext context)
        {
            if (IsFieldAccess || IsPointerFieldAccess || IsArrayIndex)
            {
                if (Tokens[0] is FunctionCallToken)
                {
                    throw new Exception("Postfix operators not (yet?) supported on function calls");
                }

                EmitAddress(context);
                
                //variable address -> eax
                context.EmitInstruction(new IRPop() { To = "eax" });

                int valueSize = GetExpressionType(context).GetSize();

                if (valueSize > 4)
                {
                    context.EmitInstruction(new IRMemCopy() { From = "eax", To = "sp", Length = new ImmediateValue(valueSize) });
                    context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(valueSize) });
                    context.EmitInstruction(new IRAdd() { Left = "sp", Right = "ebx", To = "sp" });
                }
                else
                {
                    context.EmitInstruction(new IRLoadRegister() { From = "eax", To = "ebx", OperandSize = valueSize }); //MB!
                    context.EmitInstruction(new IRPushRegister() { From = "ebx" });
                }
            }
            else
            {
                ((ICodeEmitter)Tokens[0]).Emit(context);
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (IsFieldAccess || IsPointerFieldAccess)
            {
                string fieldName = ((IdentifierToken)Tokens[2]).Name;

                var leftHandSideType = ((IHasType)Tokens[0]).GetExpressionType(context);

                if (IsFieldAccess)
                {
                    if (leftHandSideType.IndirectionLevel != 0)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }
                }
                else if (IsPointerFieldAccess)
                {
                    if (leftHandSideType.IndirectionLevel != 1)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }
                }
                else
                {
                    throw new Exception("This shouldn't happen");
                }

                if (leftHandSideType.Type is CompositeTypeDef)
                {
                    var compTypeDef = (CompositeTypeDef)leftHandSideType.Type;

                    if (compTypeDef.Fields.ContainsKey(fieldName))
                    {
                        return compTypeDef.Fields[fieldName].Type;
                    }
                    else
                    {
                        throw new UndefinedVariableException(leftHandSideType.Type.Name + "." + fieldName);
                    }
                }
                else
                {
                    throw new UndefinedVariableException(leftHandSideType.Type.Name + "." + fieldName);
                }
            }
            else if (IsArrayIndex)
            {
                var type = ((IHasType)Tokens[0]).GetExpressionType(context);

                return new ExpressionType()
                {
                    Type = type.Type,
                    IndirectionLevel = type.IndirectionLevel - 1, //array access dereferences pointer
                };
            }
            else
            {
                return ((IHasType)Tokens[0]).GetExpressionType(context);
            }
        }

        public void EmitAddress(CompilationContext context)
        {
            if (IsFieldAccess || IsPointerFieldAccess || IsArrayIndex)
            {
                ((IHasAddress)Tokens[0]).EmitAddress(context);

                //Address of left hand side -> eax
                context.EmitInstruction(new IRPop() { To = "eax" });

                string fieldName = "";

                var leftHandSideType = ((IHasType)Tokens[0]).GetExpressionType(context);

                if (IsFieldAccess || IsPointerFieldAccess)
                {
                    fieldName = ((IdentifierToken)Tokens[2]).Name;
                }

                if (IsFieldAccess)
                {
                    if (leftHandSideType.IndirectionLevel != 0)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }
                }
                else if (IsPointerFieldAccess)
                {
                    if (leftHandSideType.IndirectionLevel != 1)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }

                    //Dereference pointer -> eax
                    context.EmitInstruction(new IRLoadRegister() { From = "eax", To = "ebx", OperandSize = 4 });
                    context.EmitInstruction(new IRMoveRegister() { From = "ebx", To = "eax" });
                }
                else if (IsArrayIndex)
                {
                    if (leftHandSideType.IndirectionLevel < 1)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "[]");
                    }
                }
                else
                {
                    throw new Exception("This shouldn't happen");
                }

                if (IsFieldAccess || IsPointerFieldAccess)
                {
                    if (leftHandSideType.Type is CompositeTypeDef)
                    {
                        var compTypeDef = (CompositeTypeDef)leftHandSideType.Type;

                        if (compTypeDef.Fields.ContainsKey(fieldName))
                        {
                            int offset = compTypeDef.Fields[fieldName].Offset;

                            //ecx = variable address + offset
                            context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(offset) });
                            context.EmitInstruction(new IRAdd() { Left = "eax", Right = "ebx", To = "ecx" });
                            context.EmitInstruction(new IRPushRegister() { From = "ecx" });
                        }
                        else
                        {
                            throw new UndefinedVariableException(leftHandSideType.Type.Name + "." + fieldName);
                        }
                    }
                    else
                    {
                        throw new UndefinedVariableException(leftHandSideType.Type.Name + "." + fieldName);
                    }
                }
                else if (IsArrayIndex)
                {
                    //Need to get size of underlying data type, not size of whole array
                    int valueSize = leftHandSideType.GetDereferencedSize();

                    //Get index value
                    ((ICodeEmitter)Tokens[Tokens.Count - 2]).Emit(context);
                    context.EmitInstruction(new IRPop() { To = "ebx" });

                    //eax = pointer address, ebx = array index, ecx = type size
                    context.EmitInstruction(new IRMoveImmediate() { To = "ecx", Value = new ImmediateValue(valueSize) });

                    //ebx = index * size
                    context.EmitInstruction(new IRMult() { Left = "ebx", Right = "ecx", To = "ebx" });

                    //ecx = address + index * size
                    context.EmitInstruction(new IRAdd() { Left = "eax", Right = "ebx", To = "ecx" });
                    
                    //push address
                    context.EmitInstruction(new IRPushRegister() { From = "ecx" });
                }
            }
            else
            {
                ((IHasAddress)Tokens[0]).EmitAddress(context);
            }
        }
    }
}
