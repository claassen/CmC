using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("REGEX:NUMBER", "'[0-9]+'")]
    public class NumberToken : IUserLanguageTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public int Value;

        public override IUserLanguageToken Create(string expressionValue)
        {
            return new NumberToken() { Value = Int32.Parse(expressionValue) };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitInstruction(new Op() { Name = "push", Imm = new ImmediateValue(Value) });
        }

        public Type GetExpressionType(CompilationContext context)
        {
            return new Type() { Name = "int" };
        }

        public void EmitAddress(CompilationContext context)
        {
            throw new Exception("Can't take address of number literal");
        }
    }
}
