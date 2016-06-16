using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.Syntax.TokenInterfaces;
using ParserGen.Parser.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Syntax.Assembly
{
    [TokenExpression("CALL", "'call' IMMEDIATE")]
    public class Call : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new Call() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            ImmediateValue value = ((ImmediateValueToken)Tokens[1]).GetValue(context);

            context.EmitInstruction(new IRCallImmediate() { Address = value }); 
        }


        public int GetSizeOfAllLocalVariables(CompilationContext context)
        {
            throw new NotImplementedException();
        }
    }
}
