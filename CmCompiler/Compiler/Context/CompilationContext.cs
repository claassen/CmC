﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmC.Compiler.Exceptions;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.Context
{
    public class CompilationContext
    {
        public Dictionary<string, StringConstant> StringConstants;
        public Stack<int> ConditionalElseBranchLabels;
        public bool IsEntryPointFunction;

        private List<IRInstruction> _instructions;
        private Dictionary<string, TypeDef> _types;
        private Dictionary<string, Function> _functionSymbolTable;
        private Dictionary<string, Variable> _globalVarSymbolTable;
        private Dictionary<string, Variable>[] _varSymbolTables;
        private Dictionary<string, int> _namedLabels;
        private int _currentScopeLevel;
        private Stack<int> _stackOffsets;
        private int _functionArgBPOffset;
        private int _functionLocalVarSize;
        private int _labelCount;
        private bool _functionHasReturn;
        private bool _inPossiblyNonExecutedBlock;

        public CompilationContext()
        {
            StringConstants = new Dictionary<string, StringConstant>();
            ConditionalElseBranchLabels = new Stack<int>();

            _instructions = new List<IRInstruction>();
            _types = new Dictionary<string, TypeDef>();
            _functionSymbolTable = new Dictionary<string, Function>();
            _globalVarSymbolTable = new Dictionary<string, Variable>();
            _varSymbolTables = new Dictionary<string, Variable>[100];
            _namedLabels = new Dictionary<string, int>();

            Init();
        }

        private void Init()
        {
            //0 = global scope
            _varSymbolTables[0] = new Dictionary<string, Variable>();
            _currentScopeLevel = 0;

            _stackOffsets = new Stack<int>();
            _stackOffsets.Push(0);

            _types.Add("byte", new TypeDef() { Name = "byte", Size = 1 });
            _types.Add("int", new TypeDef() { Name = "int", Size = 4 });
            _types.Add("bool", new TypeDef() { Name = "bool", Size = 4 });
        }

        public void EmitLabel(int labelIndex, string labelName = "")
        {
            _instructions.Add(new IRLabel(labelIndex));

            if (!String.IsNullOrEmpty(labelName))
            {
                _namedLabels.Add(labelName, labelIndex);
            }
        }

        public int GetLabelIndex(string labelName)
        {
            return _namedLabels[labelName];
        }

        public void EmitInstruction(IRInstruction ir)
        {
            _instructions.Add(ir);
        }

        public void EmitComment(string text)
        {
            _instructions.Add(new IRComment(text));
        }

        public void NewScope(bool isFunction)
        {
            _currentScopeLevel++;

            _varSymbolTables[_currentScopeLevel] = new Dictionary<string, Variable>();

            if (isFunction)
            {
                _stackOffsets.Push(0);
                _functionArgBPOffset = 0;
                _functionLocalVarSize = 0;
                _functionHasReturn = false;
                _inPossiblyNonExecutedBlock = false;
            }
        }

        public void EndScope(bool isFunction)
        {
            _currentScopeLevel--;

            if (isFunction)
            {
                _stackOffsets.Pop();
            }
        }

        public void StartPossiblyNonExecutedBlock()
        {
            _inPossiblyNonExecutedBlock = true;
        }

        public void EndPossiblyNonExecutedBlock()
        {
            _inPossiblyNonExecutedBlock = false;
        }

        public void ReportReturnStatement()
        {
            if (!_inPossiblyNonExecutedBlock)
            {
                _functionHasReturn = true;
            }
        }

        public bool FunctionHasReturn()
        {
            return _functionHasReturn;
        }

        public void AddVariableSymbol(string name, ExpressionType type, bool isStatic, bool isExported, bool isExtern)
        {
            if (isStatic)
            {
                if (_currentScopeLevel != 0)
                {
                    throw new Exception("Cannot use static storage duration on non-global variables");
                }

                //Global scope
                _globalVarSymbolTable.Add(
                    name, 
                    new Variable() 
                    { 
                        Type = type,
                        IsExported = isExported,
                        IsExtern = isExtern,
                        Address = new LabelAddressValue(CreateNewLabel())
                    }
                );
            }
            else
            {
                //Function or block scope
                int currentStackOffset = _stackOffsets.Pop();
                _varSymbolTables[_currentScopeLevel].Add(
                    name, 
                    new Variable() 
                    { 
                        Address = new StackAddressValue(currentStackOffset), 
                        Type = type 
                    }
                );
                currentStackOffset += type.GetSize();
                _stackOffsets.Push(currentStackOffset);
                _functionLocalVarSize += type.GetSize();
            }
        }

        public int CreateNewLabel()
        {
            return _labelCount++;
        }

        public int GetLastCreatedLabelNumber()
        {
            return _labelCount - 1;
        }

        public int GetFunctionLocalVarSize()
        {
            return _functionLocalVarSize;
        }

        public void AddFunctionSymbol(string name, ExpressionType returnType, List<ExpressionType> parameterTypes, bool isDefined, bool isExported)
        {
            if (_functionSymbolTable.ContainsKey(name))
            {
                var function = _functionSymbolTable[name];

                if (function.IsDefined)
                {
                    throw new DuplicateFunctionDefinitionException(name);
                }
                
                //Check function signature matches previous declaration
                if (parameterTypes.Count != function.ParameterTypes.Count)
                {
                    throw new FunctionSignatureMismatchException(name);
                }

                for (int i = 0; i < function.ParameterTypes.Count; i++)
                {
                    if (!parameterTypes[i].Equals(function.ParameterTypes[i]))
                    {
                        throw new FunctionSignatureMismatchException(name);
                    }
                }
                
                if (isDefined)
                {
                    function.IsDefined = true;
                    function.IsExported = isExported;
                    //function.Address = new LabelAddressValue(CreateNewLabel());
                }
            }
            else
            {
                _functionSymbolTable.Add(
                    name,
                    new Function()
                    {
                        Address = new LabelAddressValue(CreateNewLabel()),
                        ReturnType = returnType,
                        ParameterTypes = parameterTypes,
                        IsDefined = isDefined,
                        IsExported = isExported
                    }
                );
            }
        }

        public Dictionary<string, Function> GetFunctions()
        {
            return _functionSymbolTable;
        }

        public void AddFunctionArgSymbol(string name, ExpressionType type)
        {
            _functionArgBPOffset -= type.GetSize();
            _varSymbolTables[_currentScopeLevel].Add(name, new Variable() { Address = new StackAddressValue(_functionArgBPOffset), Type = type });
        }

        public Variable GetVariable(string varName)
        {
            for (int i = _currentScopeLevel; i >= 0; i--)
            {
                if (_varSymbolTables[i].ContainsKey(varName))
                {
                    return _varSymbolTables[i][varName];
                }
            }

            if (_globalVarSymbolTable.ContainsKey(varName))
            {
                return _globalVarSymbolTable[varName];
            }

            throw new UndefinedVariableException(varName);
        }

        public Dictionary<string, Variable> GetGlobalVariables()
        {
            return _globalVarSymbolTable;
        }

        public Function GetFunction(string funcName)
        {
            if (_functionSymbolTable.ContainsKey(funcName))
            {
                return _functionSymbolTable[funcName];
            }
            else
            {
                throw new UndefinedFunctionException(funcName);
            }
        }

        public void AddStringConstant(string value)
        {
            if (!StringConstants.ContainsKey(value))
            {
                StringConstants.Add(value, new StringConstant()
                {
                    LabelAddress = CreateNewLabel(),
                    Value = value.Substring(1, value.Length - 2)
                });
            }
        }

        public int GetStringConstantLabelAddress(string value)
        {
            if (StringConstants.ContainsKey(value))
            {
                return StringConstants[value].LabelAddress;
            }
            else
            {
                throw new Exception("String constant not defined: " + value);
            }
        }

        public Dictionary<string, StringConstant> GetStringConstants()
        {
            return StringConstants;
        }

        public void AddTypeDef(TypeDef type)
        {
            if (!_types.ContainsKey(type.Name))
            {
                _types.Add(type.Name, type);
            }
            else
            {
                throw new Exception("Duplicate type definition: " + type.Name);
            }
        }

        public TypeDef GetTypeDef(string name)
        {
            if (_types.ContainsKey(name))
            {
                return _types[name];
            }
            else
            {
                throw new Exception("Undefined type: " + name);
            }
        }

        public List<IRInstruction> GetIR()
        {
            return _instructions;
        }
    }   
}
