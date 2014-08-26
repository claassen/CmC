using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmC.Exceptions;

namespace CmC.Context
{
    public class CompilationContext
    {
        private Dictionary<string, Variable>[] _varSymbolTables;
        private int _currentScopeLevel;
        private Stack<int> _stackOffsets;

        private int _functionArgStackOffset;
        private int _functionLocalVarCount;

        private int _instructionAddress;

        public int ElseIfLabelCount;

        private List<EmitToken> _instructions;

        private Dictionary<string, Function> _functionSymbolTable;
        private Dictionary<string, Variable> _globalVarSymbolTable;
        private int _globalVarOffset;

        private bool _functionHasReturn;
        private bool _inPossiblyNonExecutedBlock;

        private Dictionary<string, TypeDef> _types;

        public CompilationContext()
        {
            _varSymbolTables = new Dictionary<string, Variable>[100];
            _functionSymbolTable = new Dictionary<string, Function>();
            _globalVarSymbolTable = new Dictionary<string, Variable>();

            //Start at global scope
            _currentScopeLevel = -1;
            _varSymbolTables[0] = new Dictionary<string, Variable>();

            _stackOffsets = new Stack<int>();
            _stackOffsets.Push(0);
            
            _instructions = new List<EmitToken>();

            _types = new Dictionary<string, TypeDef>();
            _types.Add("int", new TypeDef() { Name = "int", Size = 1 });
            _types.Add("bool", new TypeDef() { Name = "bool", Size = 1 });
        }

        public void EmitLabel(string name)
        {
            _instructions.Add(new Label() { Name = name, Address = _instructionAddress });
        }

        public void EmitInstruction(Op op, string comment = null)
        {
            _instructions.Add(new Instruction() { Address = _instructionAddress++, Op = op, Comment = comment });
        }

        public void EmitComment(string text)
        {
            _instructions.Add(new Comment() { Text = text });
        }

        public int GetCurrentInstructionAddress()
        {
            return _instructionAddress - 1;
        }

        public void NewScope(bool isFunction)
        {
            _currentScopeLevel++;

            _varSymbolTables[_currentScopeLevel] = new Dictionary<string, Variable>();

            if (isFunction)
            {
                _stackOffsets.Push(0);
                _functionArgStackOffset = -1;
                _functionLocalVarCount = 0;
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

        public void AddVariableSymbol(string name, ExpressionType type)
        {
            if (_currentScopeLevel == -1)
            {
                //Global scope
                _globalVarSymbolTable.Add(name, new Variable() { Address = new StaticAddressValue(_globalVarOffset++), Type = type });
            }
            else
            {
                //Function or block scope
                int currentStackOffset = _stackOffsets.Pop();
                _varSymbolTables[_currentScopeLevel].Add(name, new Variable() { Address = new StackAddressValue(currentStackOffset++), Type = type });
                _stackOffsets.Push(currentStackOffset);
                _functionLocalVarCount++;
            }
        }

        public int GetFunctionLocalVarCount()
        {
            return _functionLocalVarCount;
        }

        public void AddFunctionSymbol(string name, ExpressionType returnType, List<ExpressionType> parameterTypes)
        {
            _functionSymbolTable.Add(name, new Function() { Address = new AbsoluteAddressValue(_instructionAddress), ReturnType = returnType, ParameterTypes = parameterTypes });
        }

        public void AddFunctionArgSymbol(string name, ExpressionType type)
        {
            _varSymbolTables[_currentScopeLevel].Add(name, new Variable() { Address = new StackAddressValue(_functionArgStackOffset--), Type = type });
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

        public void ResolveAddresses()
        {
            int codeOffset = _globalVarSymbolTable.Sum(v => v.Value.Type.GetSize());

            for (int i = 0; i < _instructions.Count; i++)
            {
                if (_instructions[i] is Label)
                {
                    ((Label)_instructions[i]).Address += codeOffset;
                }
                else if (_instructions[i] is Instruction)
                {
                    var instruction = (Instruction)_instructions[i];

                    instruction.Address += codeOffset;

                    if (instruction.Op.Imm is AbsoluteAddressValue)
                    {
                        ((AbsoluteAddressValue)instruction.Op.Imm).Number += codeOffset;
                    }
                    else if (instruction.Op.Imm is LabelAddressValue)
                    {
                        string label = ((LabelAddressValue)instruction.Op.Imm).Label;

                        bool resolved = false;

                        for (int j = i + 1; j < _instructions.Count; j++)
                        {
                            if (_instructions[j] is Label)
                            {
                                var l = (Label)_instructions[j];

                                if (l.Name == label)
                                {
                                    //Label address will be updated later in outer loop
                                    instruction.Op.Imm = new AbsoluteAddressValue(l.Address + codeOffset);
                                    resolved = true;
                                    break;
                                }
                            }
                        }

                        if (!resolved)
                        {
                            throw new Exception("Unresolved label: " + label);
                        }
                    }
                }
            }
        }

        public void PrintInstructions()
        {
            Console.WriteLine("data:");
            int addr = 0;
            foreach (var v in _globalVarSymbolTable)
            {
                Console.WriteLine(addr++ + ": " + v.Value + ": " + v.Key);
            }

            Console.WriteLine("code:");
            foreach (var i in _instructions)
            {
                Console.WriteLine(i);
            }
        }
    }   
}
