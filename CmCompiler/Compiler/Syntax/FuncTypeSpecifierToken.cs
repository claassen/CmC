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
    [TokenExpression("FUNC_TYPE_SPECIFIER", "'(' TYPE_SPECIFIER ')' '(' (TYPE_SPECIFIER (',' TYPE_SPECIFIER)*)? ')'")]
    public class FuncTypeSpecifierToken : ILanguageNonTerminalToken, IHasType
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FuncTypeSpecifierToken() { Tokens = tokens };
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            var returnType = ((TypeSpecifierToken)Tokens[0]).GetExpressionType(context);

            var argumentTypes = new List<ExpressionType>();

            for (int i = 1; i < Tokens.Count; i++)
            {
                argumentTypes.Add(((TypeSpecifierToken)Tokens[i]).GetExpressionType(context));
            }

            return new ExpressionType()
            {
                Type = new TypeDef()
                {
                    Name = "Function",
                    IsFunction = true,
                    ReturnType = returnType,
                    ArgumentTypes = argumentTypes
                },
                IndirectionLevel = 1
            };
        }
    }
}
