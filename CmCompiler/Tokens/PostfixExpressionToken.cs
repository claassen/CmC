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
    //TODO: array operator
    [TokenExpression("POSTFIX_EXPRESSION", "(FUNCTION_CALL | PRIMARY_EXPRESSION) (('.'|'->') IDENTIFIER)?")]
    public class PostfixExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new PostfixExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                if (Tokens[0] is FunctionCallToken)
                {
                    throw new Exception("Postfix operators not supported on function calls");
                }

                EmitAddress(context);

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "load", R1 = "ebx", R2 = "eax" });
                context.EmitInstruction(new Op() { Name = "push", R1 = "ebx" });
            }
            else
            {
                ((ICodeEmitter)Tokens[0]).Emit(context);
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                string fieldName = ((IdentifierToken)Tokens[2]).Name;

                var leftHandSideType = ((IHasType)Tokens[0]).GetExpressionType(context);

                string op = ((DefaultLanguageTerminalToken)Tokens[1]).Value;

                if (op == ".")
                {
                    if (leftHandSideType.IndirectionLevel != 0)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }
                }
                else if (op == "->")
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
            else
            {
                return ((IHasType)Tokens[0]).GetExpressionType(context);
            }
        }

        public void EmitAddress(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                ((IHasAddress)Tokens[0]).EmitAddress(context);

                //Address of left hand side -> eax
                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

                string fieldName = ((IdentifierToken)Tokens[2]).Name;

                var leftHandSideType = ((IHasType)Tokens[0]).GetExpressionType(context);

                string op = ((DefaultLanguageTerminalToken)Tokens[1]).Value;

                if (op == ".")
                {
                    if (leftHandSideType.IndirectionLevel != 0)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }
                }
                else if (op == "->")
                {
                    if (leftHandSideType.IndirectionLevel != 1)
                    {
                        throw new UndefinedVariableException(leftHandSideType.ToString() + "." + fieldName);
                    }

                    //Dereference pointer -> eax
                    context.EmitInstruction(new Op() { Name = "load", R1 = "ebx", R2 = "eax" });
                    context.EmitInstruction(new Op() { Name = "mov", R1 = "ebx", R2 = "eax" });
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
                        int offset = compTypeDef.Fields[fieldName].Offset;

                        //ecx = variable address + offset
                        context.EmitInstruction(new Op() { Name = "mov", R1 = "ebx", Imm = new ImmediateValue(offset) });
                        context.EmitInstruction(new Op() { Name = "add", R1 = "eax", R2 = "ebx", R3 = "ecx" });

                        context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
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
            else
            {
                ((IHasAddress)Tokens[0]).EmitAddress(context);
            }
        }
    }
}
