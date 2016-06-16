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
    [TokenExpression("BITWISE_EXPRESSION", "EQUALITY_EXPRESSION (('&'|'|'|'^') EQUALITY_EXPRESSION)*")]
    public class BitwiseExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new BitwiseExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Bitwise expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var expressionType = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (Tokens.Count > 1)
            {
                TypeChecking.CheckExpressionTypeIsNumeric(expressionType);
            }

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                expressionType = ((IHasType)Tokens[i + 1]).GetExpressionType(context);

                TypeChecking.CheckExpressionTypeIsNumeric(expressionType);

                context.EmitInstruction(new IRPop() { To = "ebx" });
                context.EmitInstruction(new IRPop() { To = "eax" });

                switch (op)
                {
                    case "&":
                        context.EmitInstruction(new IRAnd() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                    case "|":
                        context.EmitInstruction(new IROr() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                    case "^":
                        context.EmitInstruction(new IRXOr() { Left = "eax", Right = "ebx", To = "ecx" });
                        break;
                }

                context.EmitInstruction(new IRPushRegister() { From = "ecx" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                return new ExpressionType() { BaseType = context.GetTypeDef("int") };
            }
            else
            {
                return ((IHasType)Tokens[0]).GetExpressionType(context);
            }
        }

        public void PushAddress(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                throw new Exception("Can't take address of bitwise expression");
            }

            ((IHasAddress)Tokens[0]).PushAddress(context);
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
