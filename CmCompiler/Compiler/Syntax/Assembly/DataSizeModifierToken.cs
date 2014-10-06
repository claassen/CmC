using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("SIZE_MOD", "('BYTE'|'WORD')")]
    public class DataSizeModifierToken : ILanguageNonTerminalToken
    {
        public int Size;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            int size = 4;

            switch (((DefaultLanguageTerminalToken)tokens[0]).Value)
            {
                case "BYTE":
                    size = 1;
                    break;
                case "WORD":
                    size = 2;
                    break;
            }

            return new DataSizeModifierToken() { Size = size };
        }
    }
}
