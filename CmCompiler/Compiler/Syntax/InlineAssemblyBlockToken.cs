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
    [TokenExpression("INLINE_ASM", "'asm' '{' (ASM_INSTRUCTION)* '}'")]
    public class InlineAssemblyBlockToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new InlineAssemblyBlockToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
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
}
