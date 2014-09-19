using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.Common.Interface;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Common
{
    [TokenExpression("REGEX:NUM_DEC", "'([0-9]|[1-9][0-9]*)'")]
    public class DecimalNumberToken : ILanguageTerminalToken, IHasValue
    {
        public int Value;

        public override ILanguageToken Create(string expressionValue)
        {
            return new DecimalNumberToken() { Value = Convert.ToInt32(expressionValue) };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            return new ImmediateValue(Value);
        }
    }
}
