using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Tokens
{
    [TokenExpression("FUNCTION_DEFINITION", "TYPE_SPECIFIER IDENTIFIER '(' (TYPE_SPECIFIER IDENTIFIER (',' TYPE_SPECIFIER IDENTIFIER)*)? ')' FUNCTION_BODY")]
    public class FunctionDefinitionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new FunctionDefinitionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Function definition");

            var returnType = ((TypeSpecifierToken)Tokens[0]).GetExpressionType(context);

            string functionName = ((IdentifierToken)Tokens[1]).Name;

            context.NewScope(true);

            var parameterTypes = new List<ExpressionType>();

            for (int i = 2; i < Tokens.Count - 1; i += 2)
            {
                var parameterType = ((IHasType)Tokens[i]).GetExpressionType(context);

                parameterTypes.Add(parameterType);

                string parameterName = ((IdentifierToken)Tokens[i + 1]).Name;

                context.AddFunctionArgSymbol(parameterName, parameterType);
            }

            context.AddFunctionSymbol(functionName, returnType, parameterTypes);

            ((ICodeEmitter)Tokens.Last()).Emit(context);

            context.EndScope(true);

            if (!context.FunctionHasReturn())
            {
                throw new MissingReturnException(functionName);
            }
        }
    }
}
