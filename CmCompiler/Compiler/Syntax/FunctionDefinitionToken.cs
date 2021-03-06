﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.Syntax.Common;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;
using CmC.Compiler.IR;

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

            context.NewFunctionScope(returnType);

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
                //Add halt instruction to guard code from running into function code when the function was not explicitly called
                context.EmitInstruction(new IRHalt());

                context.EmitLabel(context.GetFunction(functionName).Address.Value);

                var functionBody = ((ICodeEmitter)Tokens.Last());

                int sizeOfAllLocalVariables = functionBody.GetSizeOfAllLocalVariables(context);

                context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = new ImmediateValue(sizeOfAllLocalVariables) });
                context.EmitInstruction(new IRAdd() { Left = "sp", Right = "eax", To = "sp" });

                //Function body
                functionBody.Emit(context);

                if (!context.FunctionHasReturn())
                {
                    if (returnType.GetSize() > 0)
                    {
                        throw new MissingReturnException(functionName);
                    }
                    else
                    {
                        new ReturnStatementToken() { Tokens = new List<ILanguageToken>() }.Emit(context);
                    }
                }
            }

            context.EndScope(true);

            context.IsEntryPointFunction = false;
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            return 0;
        }
    }
}
