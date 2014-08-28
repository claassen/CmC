using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Tokens
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

            ExpressionType.CheckTypesMatch(leftSideType, rightSideType);

            ((IHasAddress)Tokens[0]).EmitAddress(context);

            //context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
            context.EmitInstruction(new IRPop() { To = "eax" });

            ((ICodeEmitter)Tokens[2]).Emit(context);

            //Store assign value in ebx
            //context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });
            context.EmitInstruction(new IRPop() { To = "ebx" });

            //store ebx -> memory[eax]
            //context.EmitInstruction(new Op() { Name = "store", R1 = "eax", R2 = "ebx" });
            context.EmitInstruction(new IRStoreRegister() { From = "ebx", To = "eax" });
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
