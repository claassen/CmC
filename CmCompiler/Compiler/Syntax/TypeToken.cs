﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Syntax.Common;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler.Syntax
{
    [TokenExpression("TYPE", "IDENTIFIER")]
    public class TypeToken : ILanguageNonTerminalToken
    {
        public override ILanguageToken Create(string expressionValue, List<ILanguageToken> tokens)
        {
            return new TypeToken() { Tokens = tokens };
        }

        public string GetTypeName()
        {
            return ((IdentifierToken)Tokens[0]).Name;
        }
    }
}
