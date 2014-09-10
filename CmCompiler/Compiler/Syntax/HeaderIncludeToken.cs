using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("INCLUDE", "'#include' STRING")]
    public class HeaderIncludeToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public string IncludePath;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new HeaderIncludeToken() { IncludePath = ((StringLiteralToken)tokens[1]).Value.Trim('"') };
        }

        public void Emit(CompilationContext context)
        {
            context.ProcessHeader(IncludePath);
        }
    }
}
