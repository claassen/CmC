using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmC.Exceptions;

namespace CmC
{
    public class CompilationContext
    {
        private Dictionary<string, int>[] _varSymbolTables;
        private int _currentScopeLevel;
        private Stack<int> _stackOffsets;

        private int _functionArgStackOffset;
        private int _functionLocalVarCount;

        private int _instructionAddress;

        public int ElseIfLabelCount;

        private List<EmitToken> _instructions;

        private Dictionary<string, int> _functionSymbolTable;
        private Dictionary<string, int> _globalVarSymbolTable;
        private int _globalVarOffset;

        private bool _functionHasReturn;
        private bool _inPossiblyNonExecutedBlock;

        public CompilationContext()
        {
            _varSymbolTables = new Dictionary<string, int>[100];
            _functionSymbolTable = new Dictionary<string, int>();
            _globalVarSymbolTable = new Dictionary<string, int>();

            //Start at global scope
            _currentScopeLevel = -1;
            _varSymbolTables[0] = new Dictionary<string, int>();

            _stackOffsets = new Stack<int>();
            _stackOffsets.Push(0);
            
            _instructions = new List<EmitToken>();
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

            _varSymbolTables[_currentScopeLevel] = new Dictionary<string, int>();

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

        public void AddVariableSymbol(string name)
        {
            if (_currentScopeLevel == -1)
            {
                //Global scope
                _globalVarSymbolTable.Add(name, _globalVarOffset++);
            }
            else
            {
                //Function or block scope
                int currentStackOffset = _stackOffsets.Pop();
                _varSymbolTables[_currentScopeLevel].Add(name, currentStackOffset++);
                _stackOffsets.Push(currentStackOffset);
                _functionLocalVarCount++;
            }
        }

        public int GetFunctionLocalVarCount()
        {
            return _functionLocalVarCount;
        }

        public void AddFunctionSymbol(string name)
        {
            _functionSymbolTable.Add(name, _instructionAddress);
        }

        public void AddFunctionArgSymbol(string name)
        {
            _varSymbolTables[_currentScopeLevel].Add(name, _functionArgStackOffset--);
        }

        public AddressValue GetVariableAddress(string varName)
        {
            for (int i = _currentScopeLevel; i >= 0; i--)
            {
                if (_varSymbolTables[i].ContainsKey(varName))
                {
                    return new StackAddressValue(_varSymbolTables[i][varName]);
                }
            }

            if (_globalVarSymbolTable.ContainsKey(varName))
            {
                return new StaticAddressValue(_globalVarSymbolTable[varName]);
            }

            throw new UndefinedVariableException(varName);
        }

        public AbsoluteAddressValue GetFunctionAddress(string funcName)
        {
            if (_functionSymbolTable.ContainsKey(funcName))
            {
                return new AbsoluteAddressValue(_functionSymbolTable[funcName]);
            }
            else
            {
                throw new UndefinedFunctionException(funcName);
            }
        }

        public void ResolveAddresses()
        {
            int codeOffset = _globalVarSymbolTable.Count;

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
            foreach (var v in _globalVarSymbolTable)
            {
                Console.WriteLine(v.Value + ": " + v.Key);
            }

            Console.WriteLine("code:");
            foreach (var i in _instructions)
            {
                Console.WriteLine(i);
            }
        }
    }

    public interface EmitToken
    {
    }

    public class Instruction : EmitToken
    {
        public int Address;
        public Op Op;
        public string Comment;

        public override string ToString()
        {
            return String.Format("{0,-30}", String.Format("{0,-2}", Address) + ": " + Op) 
                + (Comment != null ? "; " + Comment : "");
        }
    }

    public class Label : EmitToken
    {
        public string Name;
        public int Address;

        public override string ToString()
        {
            return Name + ": ";
        }
    }

    public class Comment : EmitToken
    {
        public string Text;

        public override string ToString()
        {
            return ";" + Text;
        }
    }

    public class Op
    {
        public string Name;
        public string R1;
        public string R2;
        public string R3;
        public ImmediateValue Imm;

        public override string ToString()
        {
            return Name + " " +
                (R1 != null ? R1 + ", " : "") +
                (R2 != null ? R2 + ", " : "") +
                (R3 != null ? R3 + ", " : "") +
                (Imm != null ? "$" + Imm : "");
        }
    }

    public class ImmediateValue
    {
        public int Number;

        public ImmediateValue(int num)
        {
            Number = num;
        }

        public override string ToString()
        {
            return Number.ToString();
        }
    }

    public abstract class AddressValue : ImmediateValue
    {
        public AddressValue(int offset)
            : base(offset)
        {
        }
    }

    public class AbsoluteAddressValue : AddressValue
    {
        public AbsoluteAddressValue(int offset)
            : base(offset)
        {
        }
    }

    public class StaticAddressValue : AddressValue
    {
        public StaticAddressValue(int offset)
            : base(offset)
        {
        }
    }

    public class StackAddressValue : AddressValue
    {
        public string BaseRegister;

        public StackAddressValue(int offset)
            : base(offset)
        {
            BaseRegister = "bp";
        }

        public override string ToString()
        {
            return "[bp + " + Number + "]";
        }
    }

    public class LabelAddressValue : AddressValue
    {
        public string Label;

        public LabelAddressValue(string labelName)
            : base(0)
        {
            Label = labelName;
        }
    }
}
