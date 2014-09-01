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

            if (variable.Type.GetSize() > 4)
            {
                //Variable address -> eax
                context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = variable.Address });
                //[eax] -> [sp] 
                context.EmitInstruction(new IRMemCopy() { From = "eax", To = "sp", Length = new ImmediateValue(variable.Type.GetSize()) });
                //size -> ebx
                context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(variable.Type.GetSize()) });
                //sp += ebx
                context.EmitInstruction(new IRAdd() { Left = "sp", Right = "ebx", To = "sp" });
            }
            else
            {
                //[address] -> eax
                context.EmitInstruction(new IRLoadImmediate() { To = "eax", Address = variable.Address });
                context.EmitInstruction(new IRPushRegister() { From = "eax" });
            }
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            var variable = context.GetVariable(Name);

            return variable.Type;
        }

        public void EmitAddress(CompilationContext context)
        {
            var variable = context.GetVariable(Name);

            //context.EmitInstruction(new Op() { Name = "push", Imm = variable.Address });
            context.EmitInstruction(new IRPushImmediate() { Value = variable.Address });
        }
    }
}
