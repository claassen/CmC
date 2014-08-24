using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("ASSIGNMENT", "PRIMARY '=' EXPRESSION")]
    public class AssignmentToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AssignmentToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Assignment");

            ((ICodeEmitter)Tokens[2]).Emit(context);

            var leftSideType = ((IHasType)Tokens[0]).GetExpressionType(context);
            var rightSideType = ((IHasType)Tokens[2]).GetExpressionType(context);

            Type.CheckTypesMatch(leftSideType, rightSideType);

            ((IHasAddress)Tokens[0]).EmitAddress(context);

            context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

            //Store assign value in ebx
            context.EmitInstruction(new Op() { Name = "pop", R1 = "ebx" });

            //store ebx -> memory[eax]
            context.EmitInstruction(new Op() { Name = "store", R1 = "eax", R2 = "ebx" });
        }

        public Type GetExpressionType(CompilationContext context)
        {
            return ((IHasType)Tokens[2]).GetExpressionType(context);
        }

        public void EmitAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of assignment expression");
        }
    }
}
