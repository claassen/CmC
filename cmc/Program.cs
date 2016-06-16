using CmC.Compiler;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmc
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                string sourceFilePath = "";
                string objectFilePath = "";
                bool generateAssemblyFile = false;
                bool generateAssemblyComments = false;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-a")
                    {
                        generateAssemblyFile = true;
                    }
                    else if (args[i] == "-ac")
                    {
                        generateAssemblyFile = true;
                        generateAssemblyComments = true;
                    }
                    else if (args[i] == "-o")
                    {
                        i++;
                        objectFilePath = args[i];
                    }
                    else
                    {
                        sourceFilePath = args[i];
                    }
                }

                if (String.IsNullOrEmpty(sourceFilePath))
                {
                    ShowUsage();
                    return -1;
                }

                CompilationContext context = CmCompiler.CompileFile(sourceFilePath);

                objectFilePath = !String.IsNullOrEmpty(objectFilePath)
                    ? objectFilePath
                    : Path.Combine(
                        Path.GetDirectoryName(sourceFilePath),
                        Path.GetFileNameWithoutExtension(sourceFilePath) + ".o"
                    );

                using (var fs = new FileStream(objectFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    CmCompiler.CreateObjectCode(fs, new RIVMArchitecture(), context.GetIR(), context.GetStringConstants(), context.GetGlobalVariables(), context.GetFunctions());
                }

                if (generateAssemblyFile)
                {
                    string assemblyFilePath = Path.Combine(
                        Path.GetDirectoryName(sourceFilePath),
                        Path.GetFileNameWithoutExtension(sourceFilePath) + ".a"
                    );

                    using (var fs = new FileStream(assemblyFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        CmCompiler.GenerateAssemblyOutput(fs, context.GetIR(), generateAssemblyComments);
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return -1;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: cmc <source file path> [ -a | -ac ] [ -o <object file path>");
            Console.WriteLine("Flags:");
            Console.WriteLine(" -a    Generate assembly output file.");
            Console.WriteLine(" -a    Generate assembly output file with comments.");
        }
    }
}
