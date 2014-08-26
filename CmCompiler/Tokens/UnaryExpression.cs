﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("UNARY_EXPRESSION", "('*'|'&')? POSTFIX_EXPRESSION")]
    public class UnaryExpression : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
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

                    //Get value of expression
                    context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

                    //value at memory[value] -> ebx
                    context.EmitInstruction(new Op() { Name = "load", R1 = "ebx", R2 = "eax" });

                    context.EmitInstruction(new Op() { Name = "push", R1 = "ebx" });
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
