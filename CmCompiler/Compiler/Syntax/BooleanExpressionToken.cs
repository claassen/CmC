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
    [TokenExpression("BOOLEAN_EXPRESSION", "BITWISE_EXPRESSION (('&&'|'||') BITWISE_EXPRESSION)*")]
    public class BooleanExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new BooleanExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Boolean expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var expressionType = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (Tokens.Count > 1)
            {
                TypeChecking.CheckExpressionTypeIsBoolean(expressionType); 
            }

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                expressionType = ((IHasType)Tokens[i + 1]).GetExpressionType(context);

                TypeChecking.CheckExpressionTypeIsBoolean(expressionType);

                context.EmitInstruction(new IRPop() { To = "ebx" });
                context.EmitInstruction(new IRPop() { To = "eax" });

                var trueLabel = new LabelAddressValue(context.CreateNewLabel());

                switch (op)
                {
                    case "&&":
                        context.EmitInstruction(new IRAnd() { Left = "eax", Right = "ebx", To = "ecx" });
                        context.EmitInstruction(new IRCompareImmediate() { Left = "ecx", Right = new ImmediateValue(0) });
                        context.EmitInstruction(new IRJumpNE() { Address = trueLabel });
                        break;
                    case "||":
                        context.EmitInstruction(new IROr() { Left = "eax", Right = "ebx", To = "ecx" });
                        context.EmitInstruction(new IRCompareImmediate() { Left = "ecx", Right = new ImmediateValue(0) });
                        context.EmitInstruction(new IRJumpNE() { Address = trueLabel });
                        break;
                }

                var skipTrueLabel = new LabelAddressValue(context.CreateNewLabel());

                context.EmitInstruction(new IRPushImmediate() { Value = new ImmediateValue(0) });
                context.EmitInstruction(new IRJumpImmediate() { Address = skipTrueLabel });
                context.EmitLabel(trueLabel.Value);
                context.EmitInstruction(new IRPushImmediate() { Value = new ImmediateValue(1) });
                context.EmitLabel(skipTrueLabel.Value);
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            if (Tokens.Count > 1)
            {
                return new ExpressionType() { BaseType = context.GetTypeDef("bool") };
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
                throw new Exception("Can't take address of boolean expression");
            }

            ((IHasAddress)Tokens[0]).PushAddress(context);
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
