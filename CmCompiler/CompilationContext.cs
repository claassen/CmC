using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC
{
    public class CompilationContext
    {
        private Dictionary<string, int>[] _varSymbolTables;
        private Dictionary<string, int>[] _funcSymbolTables;
        private int _currentScopeLevel;
        private Stack<int> _stackOffsets;

        private int _functionArgStackOffset;
        private int _functionLocalVarCount;

        private int _instructionAddress;

        public int ElseIfCount;

        public CompilationContext()
        {
            _varSymbolTables = new Dictionary<string, int>[100];
            _funcSymbolTables = new Dictionary<string, int>[100];
            _currentScopeLevel = 0;
            _stackOffsets = new Stack<int>();
            _stackOffsets.Push(0);
            _varSymbolTables[0] = new Dictionary<string, int>();
            _funcSymbolTables[0] = new Dictionary<string, int>();
        }

        public void Emit(string instruction, string comment = null)
        {
            Console.WriteLine(
                String.Format("{0,-30}", String.Format("{0,-2}", _instructionAddress++) + ": " + instruction)
                + (comment != null ? "; " + comment : "")
            );
        }

        public int GetCurrentInstructionAddress()
        {
            return _instructionAddress - 1;
        }

        public void NewScope(bool isFunction)
        {
            _currentScopeLevel++;
            _varSymbolTables[_currentScopeLevel] = new Dictionary<string, int>();
            _funcSymbolTables[_currentScopeLevel] = new Dictionary<string, int>();

            if (isFunction)
            {
                _stackOffsets.Push(0);
                _functionArgStackOffset = -1;
                _functionLocalVarCount = 0;
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

        public void AddVariableSymbol(string name)
        {
            int currentStackOffset = _stackOffsets.Pop();
            _varSymbolTables[_currentScopeLevel].Add(name, currentStackOffset);
            currentStackOffset++;
            _stackOffsets.Push(currentStackOffset);
            _functionLocalVarCount++;
        }

        public int GetFunctionLocalVarCount()
        {
            return _functionLocalVarCount;
        }

        public void AddFunctionSymbol(string name)
        {
            _funcSymbolTables[_currentScopeLevel].Add(name, _instructionAddress);
        }

        public void AddFunctionArgSymbol(string name)
        {
            _varSymbolTables[_currentScopeLevel].Add(name, _functionArgStackOffset);
            _functionArgStackOffset--;
        }

        public int? GetVariableAddress(string varName)
        {
            for (int i = _currentScopeLevel; i >= 0; i--)
            {
                if (_varSymbolTables[i].ContainsKey(varName))
                {
                    return _varSymbolTables[i][varName];
                }
            }

            return null;
        }

        public int? GetFunctionAddress(string funcName)
        {
            for (int i = _currentScopeLevel; i >= 0; i--)
            {
                if (_funcSymbolTables[i].ContainsKey(funcName))
                {
                    return _funcSymbolTables[i][funcName];
                }
            }

            return null;
        }
    }
}
