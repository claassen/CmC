using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Context;
using CmC.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Tokens
{
    [TokenExpression("VARIABLE_DEFINITION", "TYPE_SPECIFIER IDENTIFIER ('=' EXPRESSION)?")]
    public class VariableDefinitionToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new VariableDefinitionToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            context.EmitComment(";Variable definition");

            var type = ((IHasType)Tokens[0]).GetExpressionType(context);

            string variableName = ((IdentifierToken)Tokens[1]).Name;

            context.AddVariableSymbol(variableName, type);

            if (Tokens.Count > 2)
            {
                ((ICodeEmitter)Tokens[3]).Emit(context);

                var expressionType = ((IHasType)Tokens[3]).GetExpressionType(context);

                var variable = context.GetVariable(variableName);

                ExpressionType.CheckTypesMatch(variable.Type, expressionType);

                context.EmitInstruction(new Op() { Name = "pop", R1 = "eax" });
                context.EmitInstruction(new Op() { Name = "store", R1 = "eax", Imm = variable.Address });
            }
        }
    }
}
