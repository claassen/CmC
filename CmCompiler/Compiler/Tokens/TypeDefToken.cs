using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Tokens
{
    [TokenExpression("TYPEDEF", "'typedef' IDENTIFIER '{' (TYPE_SPECIFIER IDENTIFIER ';')+ '}'")]
    public class TypeDefToken : ILanguageNonTerminalToken, ICodeEmitter
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new TypeDefToken() { Tokens = tokens };
        }

        public void Emit(CompilationContext context)
        {
            string name = ((IdentifierToken)Tokens[1]).Name;

            var fields = new Dictionary<string, Field>();

            int offset = 0;

            for(int i = 3; i < Tokens.Count - 1; i += 2)
            {
                var type = ((TypeSpecifierToken)Tokens[i]).GetExpressionType(context);
                string fieldName = ((IdentifierToken)Tokens[i + 1]).Name;

                fields.Add(fieldName, new Field() { Offset = offset, Type = type });

                offset += type.GetSize();
            }

            var typeDef = new CompositeTypeDef()
            {
                Name = name,
                Fields = fields
            };

            context.AddTypeDef(typeDef);
        }
    }
}
