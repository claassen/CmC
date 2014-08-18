using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [UserLanguageToken("BOOLEAN_EXPRESSION", "EQUALITY_EXPRESSION (('&&'|'||'|'^') EQUALITY_EXPRESSION)*")]
    public class BooleanExpressionToken : IUserLanguageNonTerminalToken, ICodeEmitter
    {
        public override IUserLanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new BooleanExpressionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            Console.WriteLine(";Boolean expression");

            ((ICodeEmitter)Tokens[0]).Emit(context);

            for (int i = 1; i < Tokens.Count; i += 2)
            {
                ((ICodeEmitter)Tokens[i + 1]).Emit(context);

                string op = ((DefaultLanguageTerminalToken)Tokens[i]).Value;

                context.Emit("pop -> eax");
                context.Emit("pop -> ebx");

                int currentInstrAddress = context.GetCurrentInstructionAddress();
                int trueJmpLocation = currentInstrAddress + 6;

                switch (op)
                {
                    case "&&":
                        context.Emit("and eax, ebx - > ecx");
                        context.Emit("cmp ecx, $0");
                        context.Emit("jne " + trueJmpLocation);
                        break;
                    case "||":
                        context.Emit("or eax, ebx -> ecx");
                        context.Emit("cmp ecx, $0");
                        context.Emit("jne " + trueJmpLocation);
                        break;
                    case "^":
                        context.Emit("xor eax, ebx -> ecx");
                        context.Emit("cmp ecx, $0");
                        context.Emit("jne " + trueJmpLocation);
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
