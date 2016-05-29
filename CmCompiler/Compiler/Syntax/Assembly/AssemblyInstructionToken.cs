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
    [TokenExpression("ASM_INSTRUCTION", "(LABEL|ADD|AND|CALL|CMP|CPY|DIV|HALT|JMP|LOAD|MOV|MULT|NOP|OR|POP|PUSH|STORE|SUB|INT|IRET|XOR|CLI|STI|SETIDT|SETPT|TLBI|BRK)+")]
    public class AssemblyInstructionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new AssemblyInstructionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            foreach (var token in Tokens)
            {
                if (token is ICodeEmitter)
                {
                    ((ICodeEmitter)token).Emit(context);
                }
            }
        }
    }
}
