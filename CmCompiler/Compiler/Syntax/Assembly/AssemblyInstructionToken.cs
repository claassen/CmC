using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("ASM_INSTRUCTION", "(LABEL|ADD|AND|CMP|CPY|DIV|HALT|JMP|LOAD|MOV|MULT|NOP|OR|POP|PUSH|STORE|SUB|SYSENT|SYSEX|XOR)")]
    public class AssemblyInstructionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AssemblyInstructionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            ((ICodeEmitter)Tokens[0]).Emit(context);
        }
    }
}
