using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("REGEX:VARIABLE", "'[a-zA-Z]+'")]
    public class VariableToken : IUserLanguageTerminalToken, ICodeEmitter
    {
        public string Name;

        public override IUserLanguageToken Create(string expressionValue)
        {
            return new VariableToken() { Name = expressionValue };
        }

        public void Emit(CompilationContext context)
        {
            int? address = context.GetVariableAddress(Name);

            if (address == null)
            {
                throw new Exception("Undefined variable: " + Name);
            }

            context.Emit("load eax <- " + address);
            context.Emit("push eax");
        }
    }
}
