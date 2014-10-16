using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Exceptions;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.Common;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("VARIABLE_DEFINITION", "('static'|'export'|'extern')? TYPE_SPECIFIER IDENTIFIER ('=' EXPRESSION)?")]
    public class VariableDefinitionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        private bool IsExported;
        private bool IsExtern;
        private bool IsStatic;
        
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            if (tokens[0] is DefaultLanguageTerminalToken)
            {
                string storageModifier = ((DefaultLanguageTerminalToken)tokens[0]).Value;

                bool isStatic = false;
                bool isExtern = false; 
                bool isExported = false;

                if (storageModifier == "static")
                {
                    isStatic = true;
                }
                else if (storageModifier == "extern")
                {
                    isStatic = true;
                    isExtern = true;
                }
                else if (storageModifier == "export")
                {
                    isStatic = true;
                    isExported = true;
                }
                else
                {
                    throw new Exception("This shouldn't happen");
                }

                return new VariableDefinitionToken()
                {
                    IsStatic = isStatic,
                    IsExtern = isExtern,
                    IsExported = isExported,
                    Tokens = tokens.Skip(1).ToList(),
                };
            }
            
            return new VariableDefinitionToken() 
            { 
                Tokens = tokens, 
            };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Variable definition");

            var type = ((IHasType)Tokens[0]).GetExpressionType(context);
            string variableName = ((IdentifierToken)Tokens[1]).Name;

            if (type.IsArray && type.ArrayLength == -1)
            {
                throw new MissingArraySizeSpecifierException(variableName);
            }

            context.AddVariableSymbol(variableName, type, IsStatic, IsExported, IsExtern);

            if (!IsStatic)
            {
                context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = new ImmediateValue(type.GetSize()) });
                context.EmitInstruction(new IRAdd() { Left = "sp", Right = "eax", To = "sp" });
            }

            if (Tokens.Count > 2)
            {
                var expressionType = ((IHasType)Tokens[3]).GetExpressionType(context);

                var variable = context.GetVariable(variableName);

                if (variable.Type.GetSize() == 0)
                {
                    throw new VoidAssignmentException("to");
                }
                else if (expressionType.GetSize() == 0)
                {
                    throw new VoidAssignmentException("from");
                }

                ExpressionType.CheckTypesMatch(variable.Type, expressionType);

                if (expressionType.GetSize() > 4)
                {
                    //Memory copy

                    //Dest address -> eax
                    context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = variable.Address }); //STACK!!!!

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

                    if (variable.Address is StackAddressValue)
                    {
                        context.EmitInstruction(new IRStoreRegisterPlusImmediate() { From = "eax", To = "bp", Offset = new ImmediateValue(variable.Address.Value), OperandSize = expressionType.GetSize() });
                    }
                    else
                    {
                        context.EmitInstruction(new IRStoreImmediate() { From = "eax", To = variable.Address, OperandSize = expressionType.GetSize() });
                    }
                }
            }
        }
    }
}
