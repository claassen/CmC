using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("REGEX:IMM_LABEL", "':[A-Z]+'")]
    public class LabelAddressToken : ILanguageTerminalToken
    {
        public string Name;

        public override ILanguageToken Create(string expressionValue)
        {
            return new LabelAddressToken() { Name = expressionValue.Substring(1) };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            return new LabelAddressValue(context.GetLabelIndex(Name));
        }
    }
}
