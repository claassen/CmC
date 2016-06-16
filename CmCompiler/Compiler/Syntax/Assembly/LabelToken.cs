using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("REGEX:LABEL", "'[a-zA-Z_][a-zA-Z0-9_]*:'")]
    public class LabelToken : ILanguageTerminalToken, ICodeEmitter
    {
        public string Name;

        public override ILanguageToken Create(string expressionValue)
        {
            return new LabelToken() { Name = expressionValue.Remove(expressionValue.Length - 1, 1) };
        }

        public void Emit(CompilationContext context)
        {
            int labelIndex = context.CreateNewLabel();

            context.EmitLabel(labelIndex, Name);
        }


        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
