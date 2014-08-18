using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("EQUALITY_EXPRESSION", "ADDITIVE_EXPRESSION (('=='|'!='|'<'|'>'|'<='|'>=') ADDITIVE_EXPRESSION)*")]
    public class EqualityExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new EqualityExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine(";Equality expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.Emit("pop -> eax");
                context.Emit("pop -> ebx");

                context.Emit("cmp eax, ebx");

                int currentInstrAddress = context.GetCurrentInstructionAddress();
                int trueJmpLocation = currentInstrAddress + 4;

                switch (op)
                {
                    case "==":
                        context.Emit("je " + trueJmpLocation);
                        break;
                    case "!=":
                        context.Emit("jne " + trueJmpLocation);
                        break;
                    case ">":
                        context.Emit("jl " + trueJmpLocation);
                        break;
                    case "<":
                        context.Emit("jg " + trueJmpLocation);
                        break;
                    case ">=":
                        context.Emit("jge " + trueJmpLocation);
                        break;
                    case "<=":
                        context.Emit("jle " + trueJmpLocation);
                        break;
                }

                context.Emit("FALSE: push $0");
                context.Emit("jmp " + (trueJmpLocation + 1));
                context.Emit("TRUE: push $1");
                context.Emit("DONE: nop");
            }
        }
    }
}
