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
    //TODO: checking equality of expressions larger than 4 bytes
    [TokenExpression("EQUALITY_EXPRESSION", "ADDITIVE_EXPRESSION (('=='|'!='|'<'|'>'|'<='|'>=') ADDITIVE_EXPRESSION)?")]
    public class EqualityExpressionToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new EqualityExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Equality expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            var t1 = ((IHasType)Tokens[0]).GetExpressionType(context);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                var t2 = ((IHasType)Tokens[i + 1]).GetExpressionType(context);

                ExpressionType.CheckTypesMatch(t1, t2);
                t1 = t2;

                context.EmitInstruction(new IRPop() { To = "ebx" });
                context.EmitInstruction(new IRPop() { To = "eax" });

                context.EmitInstruction(new IRCompareRegister() { Left = "eax", Right = "ebx" });

                var trueLabel = new LabelAddressValue(context.CreateNewLabel());

                switch (op)
                {
                    case "==":
                        context.EmitInstruction(new IRJumpEQ() { Address = trueLabel });
                        break;
                    case "!=":
                        context.EmitInstruction(new IRJumpNE() { Address = trueLabel });
                        break;
                    case ">":
                        context.EmitInstruction(new IRJumpGT() { Address = trueLabel });
                        break;
                    case "<":
                        context.EmitInstruction(new IRJumpLT() { Address = trueLabel });
                        break;
                    case ">=":
                        context.EmitInstruction(new IRJumpGE() { Address = trueLabel });
                        break;
                    case "<=":
                        context.EmitInstruction(new IRJumpLE() { Address = trueLabel });
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
                return new ExpressionType() { Type = context.GetTypeDef("bool") };
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
                throw new Exception("Can't take address of equality expression");
            }

            ((IHasAddress)Tokens[0]).EmitAddress(context);
        }
    }
}
