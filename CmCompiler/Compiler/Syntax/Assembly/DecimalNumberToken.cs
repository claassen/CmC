using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("REGEX:IMM_DEC", "'\\$([0-9]|[1-9][0-9]*)'")]
    public class DecimalNumberToken : ILanguageTerminalToken
    {
        public int Value;

        public override ILanguageToken Create(string expressionValue)
        {
            return new DecimalNumberToken() { Value = Convert.ToInt32(expressionValue.Remove(0, 1)) };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            return new ImmediateValue(Value);
        }
    }
}
