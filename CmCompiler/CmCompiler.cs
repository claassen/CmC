using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ParserGen.Generator;
using ParserGen.Parser;

namespace CmC
{
    public class CmCompiler
    {
        private CompilationContext _context;

        public void Compile(string source)
        {
            var grammar = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.Namespace == "CmC.Tokens")
                .Select(t => (IUserLanguageToken)Activator.CreateInstance(t, null));

            var generator = new ParserGenerator(grammar.ToList());

            var parser = generator.GetParser();

            var tokens = parser.Parse(source);

            _context = new CompilationContext();

            foreach (var token in tokens)
            {
                ((ICodeEmitter)token).Emit(_context);
            }
        }
    }

    public class CompilationContext
    {
        public List<Dictionary<string, int>> SymbolTables;
        public int CurrentScopeLevel;

        public CompilationContext()
        {
            SymbolTables = new List<Dictionary<string, int>>();
            CurrentScopeLevel = 0;
        }
    }
}
