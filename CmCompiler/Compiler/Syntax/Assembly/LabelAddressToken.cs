using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using ParserGen.Parser.Tokens;
using CmC.Compiler.Syntax.Common.Interface;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("REGEX:IMM_LABEL", "':[a-zA-Z_][a-zA-Z0-9_]*'")]
    public class LabelAddressToken : ILanguageTerminalToken, IHasValue
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
