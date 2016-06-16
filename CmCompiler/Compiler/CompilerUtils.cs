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

        public static void ProcessIncludes(ref string source, string sourceFolder, HashSet<String> alreadyIncludedFiles = null)
        {
            if (alreadyIncludedFiles == null)
            {
                alreadyIncludedFiles = new HashSet<string>();
            }

            var matches = Regex.Matches(source, "#include (\".*\")");

            List<String> includePaths = new List<string>();

            for (int i = 0; i < matches.Count; i++)
            {
                string includePath = matches[i].Groups[1].Value.Trim('"');
                
                if(!Path.IsPathRooted(includePath))
                {
                    includePath = Path.Combine(sourceFolder, includePath);
                }

                includePaths.Add(includePath);
                
                source = source.Replace("#include " + matches[i].Groups[1].Value, "");
            }

            string includedSource = "";

            foreach (String path in includePaths)
            {
                using (var stream = new StreamReader(new FileStream(path, FileMode.Open)))
                {
                    string fileSource = stream.ReadToEnd();

                    ProcessIncludes(ref fileSource, Path.GetDirectoryName(path), alreadyIncludedFiles);

                    if (!alreadyIncludedFiles.Contains(path))
                    {
                        includedSource = includedSource + "\n" + fileSource;
                        alreadyIncludedFiles.Add(path);
                    }
                }
            }

            source = includedSource + "\n" + source;
        }

        public static void ProcessMacros(ref string source)
        {
            var macros = new Dictionary<string, string>();

            var matches = Regex.Matches(source, "#define ([^ \n\r]+) +([^\n\r]+)");

            while (matches.Count > 0)
            {
                string key = matches[0].Groups[1].Value;
                string value = matches[0].Groups[2].Value;

                source = Regex.Replace(source, "#define " + key + " +" + Regex.Escape(value), "");
                source = source.Replace(key, value);

                matches = Regex.Matches(source, "#define ([^ \n\r]+) +([^\n\r]+)");
            }
        }
    }
}
