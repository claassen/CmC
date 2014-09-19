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
    [TokenExpression("REGEX:NUM_HEX", "'0x[0-9a-fA-F]{1,8}'")]
    public class HexNumberToken : ILanguageTerminalToken, IHasValue
    {
        public int Value;

        public override ILanguageToken Create(string expressionValue)
        {
            return new HexNumberToken() { Value = Convert.ToInt32(expressionValue.Substring(2), 16) };
        }

        public ImmediateValue GetValue(CompilationContext context)
        {
            return new ImmediateValue(Value);
        }
    }
}
