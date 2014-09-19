using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Generator;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("INLINE_ASM", "'asm' '{' STRING '}'")]
    public class InlineAssemblyBlockToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public string AssemblySource;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new InlineAssemblyBlockToken() { AssemblySource = ((StringLiteralToken)tokens[2]).Value.Trim('"') };
        }

        public void Emit(CompilationContext context)
        {
            var grammar = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.Namespace == "CmC.Compiler.Syntax.Assembly" || c.Namespace == "CmC.Compiler.Syntax.Common")
                .Where(c => typeof(ILanguageToken).IsAssignableFrom(c))
                .Select(t => (ILanguageToken)Activator.CreateInstance(t, null)).ToList();

            var generator = new ParserGenerator(grammar);

            LanguageParser parser = generator.GetParser();

            var tokens = parser.Parse(AssemblySource, "ASM_INSTRUCTION");

            foreach (var token in tokens)
            {
                ((ICodeEmitter)token).Emit(context);
            }

            //foreach (var token in Tokens)
            //{
            //    if (token is ICodeEmitter)
            //    {
            //        ((ICodeEmitter)token).Emit(context);
            //    }
            //}
        }
    }
}
