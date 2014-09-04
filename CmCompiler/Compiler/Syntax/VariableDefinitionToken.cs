using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("VARIABLE_DEFINITION", "('export'|'extern')? TYPE_SPECIFIER ('[' NUMBER ']')? IDENTIFIER ('=' EXPRESSION)?")]
    public class VariableDefinitionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        private bool IsExported;
        private bool IsExtern;
        private bool IsArray;
        private int ArrayLength;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            bool isArray = false;
            int arrayLength = 0;

            if (tokens[0] is DefaultLanguageTerminalToken)
            {
                string storageModifier = ((DefaultLanguageTerminalToken)tokens[0]).Value;

                if (tokens.Count > 3 && tokens[3] is NumberToken)
                {
                    //Array type
                    isArray = true;
                    arrayLength = ((NumberToken)tokens[3]).Value;
                    tokens.RemoveAt(2); //[
                    tokens.RemoveAt(2); //#
                    tokens.RemoveAt(2); //]
                }

                if (storageModifier == "extern")
                {
                    return new VariableDefinitionToken()
                    {
                        IsExtern = true,
                        Tokens = tokens.Skip(1).ToList(),
                        IsArray = isArray,
                        ArrayLength = arrayLength
                    };
                }
                else if (storageModifier == "export")
                {
                    return new VariableDefinitionToken()
                    {
                        IsExported = true,
                        Tokens = tokens.Skip(1).ToList(),
                        IsArray = isArray,
                        ArrayLength = arrayLength
                    };
                }
                else
                {
                    throw new Exception("This shouldn't happen");
                }
            }
            else
            {
                if (tokens.Count > 3 && tokens[2] is NumberToken)
                {
                    //Array type
                    isArray = true;
                    arrayLength = ((NumberToken)tokens[2]).Value;
                    tokens.RemoveAt(1); //[
                    tokens.RemoveAt(1); //#
                    tokens.RemoveAt(1); //]
                }
            }

            return new VariableDefinitionToken() 
            { 
                Tokens = tokens, 
                IsArray = isArray, 
                ArrayLength = arrayLength 
            };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Variable definition");

            var type = ((IHasType)Tokens[0]).GetExpressionType(context);

            if (IsArray)
            {
                type.IsArray = true;
                type.ArrayLength = ArrayLength;
                type.IndirectionLevel++;
            }

            string variableName = ((IdentifierToken)Tokens[1]).Name;

            context.AddVariableSymbol(variableName, type, IsExported, IsExtern);

            if (Tokens.Count > 2)
            {
                var expressionType = ((IHasType)Tokens[3]).GetExpressionType(context);

                var variable = context.GetVariable(variableName);

                ExpressionType.CheckTypesMatch(variable.Type, expressionType);

                if (expressionType.GetSize() > 4)
                {
                    //Memory copy

                    //Dest address -> eax
                    context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = variable.Address });

                    //Source address -> ebx
                    ((IHasAddress)Tokens[3]).EmitAddress(context);

                    context.EmitInstruction(new IRPop() { To = "ebx" });

                    context.EmitInstruction(new IRMemCopy() { From = "ebx", To = "eax", Length = new ImmediateValue(expressionType.GetSize()) });
                }
                else
                {
                    //Copy using register
                    ((ICodeEmitter)Tokens[3]).Emit(context);

                    context.EmitInstruction(new IRPop() { To = "eax" });

                    context.EmitInstruction(new IRStoreImmediate() { From = "eax", To = variable.Address, OperandBytes = expressionType.GetSize() });
                }
            }
        }
    }
}
