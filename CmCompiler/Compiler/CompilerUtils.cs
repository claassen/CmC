using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CmC.Compiler
{
    public static class CompilerUtils
    {
        public static void RemoveComments(ref string source)
        {
            Regex commentsRegex = new Regex("//.*(\r\n|\n)");

            source = commentsRegex.Replace(source, "\n");
        }

        public static void ProcessIncludes(ref string source, string sourceFolder)
        {
            var matches = Regex.Matches(source, "#include (\".*\")");

            for (int i = 0; i < matches.Count; i++)
            {
                string includePath = matches[i].Groups[1].Value.Trim('"');
                
                if(!Path.IsPathRooted(includePath))
                {
                    includePath = Path.Combine(sourceFolder, includePath);
                }
                
                using (var stream = new StreamReader(new FileStream(includePath, FileMode.Open)))
                {
                    string includedSource = stream.ReadToEnd();

                    ProcessIncludes(ref includedSource, Path.GetDirectoryName(includePath));

                    source = source.Replace("#include " + matches[i].Groups[1].Value, "");
                    source = includedSource + "\n" + source;
                }
            }
        }

        public static void ProcessMacros(ref string source)
        {
            var macros = new Dictionary<string, string>();

            var matches = Regex.Matches(source, "#define ([^ \n\r]+) ([^ \n\r]+)");

            for (int i = 0; i < matches.Count; i++)
            {
                macros.Add(matches[i].Groups[1].Value, matches[i].Groups[2].Value);
            }

            foreach (var macro in macros)
            {
                source = source.Replace("#define " + macro.Key + " " + macro.Value, "");
                source = source.Replace(macro.Key, macro.Value);
            }
        }
    }
}
