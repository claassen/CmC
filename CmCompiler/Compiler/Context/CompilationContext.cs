using System;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<string, Variable>[] _varSymbolTables;
        private int _currentScopeLevel;
        private Stack<int> _stackOffsets;

        private int _functionArgStackOffset;
        private int _functionLocalVarCount;

        private int _labelCount;

        private List<IRInstruction> _instructions;

        private Dictionary<string, Function> _functionSymbolTable;
        private Dictionary<string, Variable> _globalVarSymbolTable;

        private bool _functionHasReturn;
        private bool _inPossiblyNonExecutedBlock;

        private Dictionary<string, TypeDef> _types;

        public Stack<int> ConditionalElseBranchLabels;

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
            
            _instructions = new List<IRInstruction>();

            _types = new Dictionary<string, TypeDef>();
            _types.Add("int", new TypeDef() { Name = "int", Size = 1 });
            _types.Add("bool", new TypeDef() { Name = "bool", Size = 1 });

            ConditionalElseBranchLabels = new Stack<int>();
        }

        public void EmitLabel(int labelIndex)
        {
            //_instructions.Add(new Label(labelNumber));
            _instructions.Add(new IRLabel(labelIndex));
        }

        public void EmitInstruction(IRInstruction ir)
        {
            //_instructions.Add(new Instruction() { Op = op, Comment = comment });
            _instructions.Add(ir);
        }

        public void EmitComment(string text)
        {
            //_instructions.Add(new Comment() { Text = text });
            _instructions.Add(new IRComment(text));
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
                _globalVarSymbolTable.Add(name, new Variable() { Address = new LabelAddressValue(CreateNewLabel()), Type = type });
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

        public int CreateNewLabel()
        {
            return _labelCount++;
        }

        public int GetLastCreatedLabelNumber()
        {
            return _labelCount - 1;
        }

        public int GetFunctionLocalVarCount()
        {
            return _functionLocalVarCount;
        }

        public void AddFunctionSymbol(string name, ExpressionType returnType, List<ExpressionType> parameterTypes)
        {
            _functionSymbolTable.Add(
                name, 
                new Function() 
                {  
                    Address = new LabelAddressValue(CreateNewLabel()),
                    ReturnType = returnType, 
                    ParameterTypes = parameterTypes 
                }
            );
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

        public List<Variable> GetGlobalVariables()
        {
            return _globalVarSymbolTable.Select(v => v.Value).ToList();
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

        public List<IRInstruction> GetIR()
        {
            return _instructions;
        }
    }   
}
