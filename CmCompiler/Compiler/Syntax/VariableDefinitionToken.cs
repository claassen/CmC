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

            if (!IsStatic && !context.InFunctionScope())
            {
                throw new Exception("Cannot define non static variable outside of function scope.");
            }

            if (Tokens.Count > 2)
            {
                var assignmentExpressionType = ((IHasType)Tokens[3]).GetExpressionType(context);

                var variable = context.GetVariable(variableName);

                if (variable.Type.GetSize() == 0)
                {
                    throw new VoidAssignmentException("to");
                }
                else if (assignmentExpressionType.GetSize() == 0)
                {
                    throw new VoidAssignmentException("from");
                }

                TypeChecking.CheckExpressionTypesMatch(variable.Type, assignmentExpressionType);

                //Special case for assignment of string literal to byte array:
                //  Don't emit string constant normally (which would add it as a string constant in the data section),
                //  instead copy the string to the memory occupied by tge byte array itself
                if (assignmentExpressionType.BaseType is StringLiteralTypeDef && variable.Type.IsArray)
                {
                    string stringLiteral = ((StringLiteralTypeDef)assignmentExpressionType.BaseType).Value;

                    if (assignmentExpressionType.ArrayLength > variable.Type.ArrayLength)
                    {
                        throw new Exception("The string '" + stringLiteral + "' is too large to be assigned by value to the left hand expression.");
                    }

                    //Put start address to copy string value to into eax
                    if (variable.Address is StackAddressValue)
                    {
                        context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(variable.Address.Value) });
                        context.EmitInstruction(new IRAdd() { To = "ebx", Left = "bp", Right = "ebx" });
                        context.EmitInstruction(new IRPushRegister() { From = "ebx" });
                    }
                    else
                    {
                        context.EmitInstruction(new IRPushImmediate() { Value = variable.Address });
                    }

                    context.EmitInstruction(new IRPop() { To = "eax" });

                    for (int i = 0; i < assignmentExpressionType.ArrayLength; i++)
                    {
                        byte charValue = i < stringLiteral.Length ? (byte)stringLiteral[i] : (byte)0;

                        //Put char value into ebx
                        context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(charValue), OperandSize = 1 });
                        //Store char value into address [eax + char offset]
                        context.EmitInstruction(new IRStoreRegisterPlusImmediate() { To = "eax", Offset = new ImmediateValue(i), From = "ebx", OperandSize = 1 });
                    }
                }
                else
                {
                    if (assignmentExpressionType.GetSize() > 4)
                    {
                        //Memory copy

                        //Dest address -> eax
                        throw new Exception("TODO: Copy large values to stack variables");
                        //context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = variable.Address }); //STACK!!!!

                        //Source address -> ebx
                        //((IHasAddress)Tokens[3]).PushAddress(context);

                        //context.EmitInstruction(new IRPop() { To = "ebx" });

                        //context.EmitInstruction(new IRMemCopy() { From = "ebx", To = "eax", Length = new ImmediateValue(assignmentExpressionType.GetSize()) });
                    }
                    else
                    {
                        //Copy using register
                        ((ICodeEmitter)Tokens[3]).Emit(context);

                        context.EmitInstruction(new IRPop() { To = "eax" });

                        if (variable.Address is StackAddressValue)
                        {
                            context.EmitInstruction(new IRStoreRegisterPlusImmediate() { From = "eax", To = "bp", Offset = new ImmediateValue(variable.Address.Value), OperandSize = assignmentExpressionType.GetSize() });
                        }
                        else
                        {
                            context.EmitInstruction(new IRStoreImmediate() { From = "eax", To = variable.Address, OperandSize = assignmentExpressionType.GetSize() });
                        }
                    }
                }
            }
        }

        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            var type = ((IHasType)Tokens[0]).GetExpressionType(context);

            string variableName = ((IdentifierToken)Tokens[1]).Name;
            return type.GetStorageSize();
        }
    }
}
