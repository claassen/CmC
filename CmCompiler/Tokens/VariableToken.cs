using System;
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
    [TokenExpression("VARIABLE", "IDENTIFIER")]
    public class VariableToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public string Name;
        public bool IsAddressOf;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            bool isAddressOf = false;

            if (tokens.Count > 1)
            {
                if (((DefaultLanguageTerminalToken)tokens[0]).Value == "&")
                {
                    isAddressOf = true;
                }
                else
                {
                    throw new Exception("This shouldn't ever happen");
                }
            }

            return new VariableToken() { Name = ((IdentifierToken)tokens.Last()).Name, IsAddressOf = isAddressOf };
        }

        public void Emit(CompilationContext context)
        {
            var variable = context.GetVariable(Name);

            context.EmitInstruction(new Op() { Name = "load", R1 = "eax", Imm = variable.Address });
            context.EmitInstruction(new Op() { Name = "push", R1 = "eax" });
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            var variable = context.GetVariable(Name);

            return variable.Type;
        }

        public void EmitAddress(CompilationContext context)
        {
            var variable = context.GetVariable(Name);

            context.EmitInstruction(new Op() { Name = "push", Imm = variable.Address });
        }
    }
}
