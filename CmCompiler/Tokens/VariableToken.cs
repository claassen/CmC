using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("VARIABLE", "('&')? IDENTIFIER")]
    public class VariableToken : IUserLanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public string Name;
        public bool IsAddressOf;

        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
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

            if (IsAddressOf)
            {
                //&variable
                if (variable.Address is StackAddressValue)
                {
                    //Calculate address by adding offset and value of bp and push on stack
                    context.EmitInstruction(new Op() { Name = "mov", R1 = "eax", Imm = new ImmediateValue(((StackAddressValue)variable.Address).Number) });
                    context.EmitInstruction(new Op() { Name = "add", R1 = "eax", R2 = "bp", R3 = "ecx" });
                    context.EmitInstruction(new Op() { Name = "push", R1 = "ecx" });
                }
                else
                {
                    context.EmitInstruction(new Op() { Name = "push", Imm = variable.Address });
                }
            }
            else
            {
                context.EmitInstruction(new Op() { Name = "load", R1 = "eax", Imm = variable.Address });

                context.EmitInstruction(new Op() { Name = "push", R1 = "eax" });
            }
        }

        public Type GetExpressionType(CompilationContext context)
        {
            var variable = context.GetVariable(Name);

            int indirectionLevel = variable.Type.IndirectionLevel;

            if (IsAddressOf)
            {
                indirectionLevel++;
            }

            return new Type() { Name = variable.Type.Name, IndirectionLevel = indirectionLevel };
        }

        public void EmitAddress(CompilationContext context)
        {
            if (IsAddressOf)
            {
                throw new Exception("Can't take address of an address");
            }

            var variable = context.GetVariable(Name);

            //variables address -> eax
            context.EmitInstruction(new Op() { Name = "push", Imm = variable.Address });
        }
    }
}
