using CmC.Linker;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cml
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                List<String> objectFilePaths = new List<string>();
                int loadAddress = 0;
                bool hasEntryPoint = false;
                string outputFilePath = "";

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-o")
                    {
                        i++;
                        outputFilePath = args[i];
                    }
                    else if (args[i] == "-l")
                    {
                        i++;
                        if (args[i].StartsWith("0x"))
                        {
                            loadAddress = Int32.Parse(args[i].Replace("0x", ""), NumberStyles.AllowHexSpecifier);
                        }
                        else
                        {
                            loadAddress = Int32.Parse(args[i]);
                        }
                    }
                    else if (args[i] == "-e")
                    {
                        hasEntryPoint = true;
                    }
                    else
                    {
                        objectFilePaths.Add(args[i]);
                    }
                }

                if (objectFilePaths.Count == 0 || String.IsNullOrEmpty(outputFilePath))
                {
                    ShowUsage();
                    return -1;
                }

                using (var fs = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    CmLinker.Link(fs, objectFilePaths, hasEntryPoint, loadAddress);
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
            Console.WriteLine("Usage: cml <object file paths...> -o <output file path> [ -l <load address> ] [ -e ]");
            Console.WriteLine("Flags:");
            Console.WriteLine(" -l    Offset all computed addresses with the given load address.");
            Console.WriteLine(" -e    Generate jump instruction at start of executable to entry point (must contain a function called main).");
        }
    }
}
