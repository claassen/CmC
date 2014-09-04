using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("FUNCTION_DEFINITION", "('export')? TYPE_SPECIFIER IDENTIFIER '(' (TYPE_SPECIFIER IDENTIFIER (',' TYPE_SPECIFIER IDENTIFIER)*)? ')' (FUNCTION_BODY|';')")]
    public class FunctionDefinitionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        private bool IsDefined;
        private bool IsExported;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            bool isExport = tokens[0] is DefaultLanguageTerminalToken
                && ((DefaultLanguageTerminalToken)tokens[0]).Value == "export";

            bool isDefined = tokens.Last() is FunctionBodyToken;

            if (!isDefined)
            {
                //Add fake body token so iteration logic for argument processing still works
                tokens.Add(new FunctionBodyToken());
            }

            return new FunctionDefinitionToken()
            {
                IsDefined = isDefined,
                IsExported = isExport,
                Tokens = isExport ? tokens.Skip(1).ToList() : tokens
            };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Function definition");

            var returnType = ((TypeSpecifierToken)Tokens[0]).GetExpressionType(context);

            if (returnType.GetSize() > 4)
            {
                throw new LargeReturnValuesNotSupportedException();
            }

            string functionName = ((IdentifierToken)Tokens[1]).Name;

            if (functionName.Equals("main", StringComparison.CurrentCultureIgnoreCase))
            {
                context.IsEntryPointFunction = true;
            }

            if (!IsDefined && IsExported)
            {
                throw new ExportUndefinedFunctionException(functionName);
            }

            context.NewScope(true);

            var parameterTypes = new List<ExpressionType>();

            for (int i = 2; i < Tokens.Count - 1; i += 2)
            {
                var parameterType = ((IHasType)Tokens[i]).GetExpressionType(context);

                parameterTypes.Add(parameterType);

                string parameterName = ((IdentifierToken)Tokens[i + 1]).Name;

                context.AddFunctionArgSymbol(parameterName, parameterType);
            }

            context.AddFunctionSymbol(functionName, returnType, parameterTypes, IsDefined, IsExported);

            if (IsDefined)
            {
                context.EmitLabel(context.GetLastCreatedLabelNumber());
                ((ICodeEmitter)Tokens.Last()).Emit(context);
            }

            context.EndScope(true);

            if (IsDefined && !context.FunctionHasReturn())
            {
                throw new MissingReturnException(functionName);
            }

            context.IsEntryPointFunction = false;
        }
    }
}
