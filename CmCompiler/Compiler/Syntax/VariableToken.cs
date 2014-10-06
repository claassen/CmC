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
    [TokenExpression("VARIABLE", "IDENTIFIER")]
    public class VariableToken : ILanguageNonTerminalToken, ICodeEmitter, IHasType, IHasAddress
    {
        public string Name;
        public bool IsAddressOf;

        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            bool isAddressOf = false;

            if (tokens.Count > 1)
            {
                if (((DefaultLanguageTerminalToken)tokens[0]).Value == "&")
                {
                    isAddressOf = true;
                }
                else
                {
                    throw new Exception("This shouldn't ever happen");
                }
            }

            return new VariableToken() { Name = ((IdentifierToken)tokens.Last()).Name, IsAddressOf = isAddressOf };
        }

        public void Emit(CompilationContext context)
        {
            try
            {
                var variable = context.GetVariable(Name);

                if (variable.Type.GetSize() > 4)
                {
                    //ARRAYS!

                    //Variable address -> eax
                    context.EmitInstruction(new IRMoveImmediate() { To = "eax", Value = variable.Address }); //STACK!!!
                    //[eax] -> [sp] 
                    context.EmitInstruction(new IRMemCopy() { From = "eax", To = "sp", Length = new ImmediateValue(variable.Type.GetSize()) });
                    //size -> ebx
                    context.EmitInstruction(new IRMoveImmediate() { To = "ebx", Value = new ImmediateValue(variable.Type.GetSize()) });
                    //sp += ebx
                    context.EmitInstruction(new IRAdd() { Left = "sp", Right = "ebx", To = "sp" });
                }
                else
                {
                    if (variable.Type.IsArray)
                    {
                        EmitAddress(context);
                    }
                    else
                    {
                        //[address] -> eax
                        if (variable.Address is StackAddressValue)
                        {
                            context.EmitInstruction(new IRLoadRegisterPlusImmediate() { To = "eax", From = "bp", Offset = new ImmediateValue(variable.Address.Value), OperandSize = variable.Type.GetSize() });
                        }
                        else
                        {
                            context.EmitInstruction(new IRLoadImmediate() { To = "eax", Address = variable.Address, OperandSize = variable.Type.GetSize() });
                        }

                        context.EmitInstruction(new IRPushRegister() { From = "eax" });
                    }
                }

                return;
            }
            catch (UndefinedVariableException)
            {
                try
                {
                    var function = context.GetFunction(Name);

                    EmitAddress(context);

                    return;
                }
                catch (UndefinedFunctionException)
                {
                }
            }

            throw new UndefinedVariableException(Name);
        }

        public ExpressionType GetExpressionType(CompilationContext context)
        {
            try
            {
                var variable = context.GetVariable(Name);

                return variable.Type;
            }
            catch (UndefinedVariableException)
            {
                try
                {
                    var function = context.GetFunction(Name);

                    return new ExpressionType()
                    {
                        IsFunction = true,
                        ReturnType = function.ReturnType,
                        ArgumentTypes = function.ParameterTypes,
                        Type = new TypeDef()
                        {
                            Name = "Function"
                        },
                        IndirectionLevel = 1
                    };
                }
                catch (UndefinedFunctionException)
                {
                }
            }

            throw new UndefinedVariableException(Name);
        }

        public void EmitAddress(CompilationContext context)
        {
            try
            {
                var variable = context.GetVariable(Name);

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

                return;
            }
            catch (UndefinedVariableException)
            {
                try
                {
                    var function = context.GetFunction(Name);

                    context.EmitInstruction(new IRPushImmediate() { Value = function.Address });

                    return;
                }
                catch (UndefinedFunctionException)
                {
                }
            }

            throw new UndefinedVariableException(Name);
        }
    }
}
