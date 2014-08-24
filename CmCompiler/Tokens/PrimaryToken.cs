using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("PRIMARY", "('*')? ('(' BOOLEAN_EXPRESSION ')' | NUMBER | FUNCTION_CALL |  VARIABLE | '-' PRIMARY)")]
    public class PrimaryToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new PrimaryToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            if (Tokens[0] is DefaultLanguageTerminalToken && ((DefaultLanguageTerminalToken)Tokens[0]).Value == "*")
            {
                for (int i = 1; i < Tokens.Count; i++)
                {
                    if (Tokens[i] is ICodeEmitter)
                    {
                        ((ICodeEmitter)Tokens[i]).Emit(context);
                    }
                }

                //Get value of expression
                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });

                //value at memory[value] -> ebx
                context.EmitInstruction(new Op() { Name = "load", R1 = "ebx", R2 = "eax" });

                context.EmitInstruction(new Op() { Name = "push", R1 = "ebx" });
            }
            else
            {
                foreach (var token in Tokens)
                {
                    if (token is ICodeEmitter)
                    {
                        ((ICodeEmitter)token).Emit(context);
                    }
                }
            }
        }

        public Type GetExpressionType(CompilationContext context)
        {
            if (Tokens[0] is DefaultLanguageTerminalToken && ((DefaultLanguageTerminalToken)Tokens[0]).Value == "*")
            {
                var type = ((IHasType)Tokens[1]).GetExpressionType(context);

                return new Type() { Name = type.Name, IndirectionLevel = type.IndirectionLevel - 1 };
            }
            else
            {
                return ((IHasType)Tokens[0]).GetExpressionType(context);
            }
        }

        public void EmitAddress(CompilationContext context)
        {
            if (Tokens[0] is DefaultLanguageTerminalToken && ((DefaultLanguageTerminalToken)Tokens[0]).Value == "*")
            {
                ((IHasAddress)Tokens[1]).EmitAddress(context);

                //Dereference: memory[eax] -> eax
                context.EmitInstruction(new Op() { Name = "load", R1 = "eax", R2 = "eax" });
                context.EmitInstruction(new Op() { Name = "push", R1 = "eax" });
            }
            else
            {
                ((IHasAddress)Tokens[0]).EmitAddress(context);
            }
        }
    }
}
