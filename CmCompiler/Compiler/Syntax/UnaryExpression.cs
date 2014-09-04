using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("UNARY_EXPRESSION", "('*'|'&')? POSTFIX_EXPRESSION")]
    public class UnaryExpression : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new UnaryExpression() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens[0] is DefaultLanguageTerminalToken)
            {
                var op = ((DefaultLanguageTerminalToken)Tokens[0]).Value;

                if (op == "&")
                {
                    ((IHasAddress)Tokens.Last()).EmitAddress(context);
                }
                else if (op == "*")
                {
                    ((ICodeEmitter)Tokens.Last()).Emit(context);

                    //Value of pointer -> eax
                    context.EmitInstruction(new IRPop() { To = "eax" });

                    //TODO: type size
                    int valueSize = GetExpressionType(context).GetSize();

                    if (valueSize > 4)
                    {
                        //[pointer] -> [sp]
                        context.EmitInstruction(new IRMemCopy() { From = "eax", To = "sp", Length = new ImmediateValue(valueSize) });

                        //sp += size of value
                        context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(valueSize) });
                        context.EmitInstruction(new IRAdd() { Left = "sp", Right = "ebx", To = "sp" });
                    }
                    else
                    {
                        //value at memory[eax] -> ebx
                        context.EmitInstruction(new IRLoadRegister() { From = "eax", To = "ebx", OperandBytes = valueSize });

                        context.EmitInstruction(new IRPushRegister() { From = "ebx" });
                    }
                }
            }
            else
            {
                ((ICodeEmitter)Tokens[0]).Emit(context);
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (Tokens[0] is DefaultLanguageTerminalToken)
            {
                var op = ((DefaultLanguageTerminalToken)Tokens[0]).Value;

                var type = ((IHasType)Tokens.Last()).GetExpressionType(context);

                if (op == "*")
                {
                    return new ExpressionType() { Type = type.Type, IndirectionLevel = type.IndirectionLevel - 1 };
                }
                else if (op == "&")
                {
                    return new ExpressionType() { Type = type.Type, IndirectionLevel = type.IndirectionLevel + 1 };
                }
                else
                {
                    throw new Exception("This shouldn't ever happen");
                }
            }
            else
            {
                return ((IHasType)Tokens[0]).GetExpressionType(context);
            }
        }

        public void EmitAddress(CompilationContext context)
        {
            if (Tokens[0] is DefaultLanguageTerminalToken)
            {
                var op = ((DefaultLanguageTerminalToken)Tokens[0]).Value;

                if (op == "*")
                {
                    //Emit the value of the variable
                    ((ICodeEmitter)Tokens.Last()).Emit(context);
                }
                else if (op == "&")
                {
                    throw new Exception("Can't take address of address");
                }
            }
            else
            {
                //Emit the address of the variable
                ((IHasAddress)Tokens[0]).EmitAddress(context);
            }
        }
    }
}
