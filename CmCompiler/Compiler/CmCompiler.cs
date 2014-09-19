using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmC.Common;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;
using CmC.Compiler.Syntax.TokenInterfaces;
using CmC.Linker;
using ParserGen.Generator;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler
{
    public static class CmCompiler
    {
        public static void CompileFile(string path, CompilerOptions options)
        {
            string objFileName = !String.IsNullOrEmpty(options.ObjectFileName) 
                ? options.ObjectFileName 
                : Path.Combine(
                    Path.GetDirectoryName(path),
                    Path.GetFileNameWithoutExtension(path) + ".o"
                );

            using (var stream = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                CompileText(stream.ReadToEnd(), objFileName, Path.GetDirectoryName(path), options.Architecture);
            }
        }

        public static void CompileText(string source)
        {
            CompileText(source, "", "", new TestArchitecture());
        }

        public static void CompileText(string source, string outputObjFile, string sourceFolder, IArchitecture architecture)
        {
            CompilerUtils.ProcessIncludes(ref source, sourceFolder);
            CompilerUtils.RemoveComments(ref source);
            CompilerUtils.ProcessMacros(ref source);

            var context = new CompilationContext();

            ProcessSourceText(source, context);

            CreateObjectFile(outputObjFile, architecture, context.GetIR(), context.GetStringConstants(), context.GetGlobalVariables(), context.GetFunctions());
        }

        public static void CompileIR(string outputObjFile, IArchitecture architecture, List<IRInstruction> ir, Dictionary<string, StringConstant> stringConstants = null, Dictionary<string, Variable> globalVariables = null, Dictionary<string, Function> functions = null)
        {
            ShowIR(ir);

            CreateObjectFile(outputObjFile, architecture, ir, stringConstants, globalVariables, functions);
        }

        internal static void ProcessSourceText(string source, CompilationContext context)
        {
            var grammar = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.Namespace == "CmC.Compiler.Syntax" || c.Namespace == "CmC.Compiler.Syntax.Common")
                .Where(c => typeof(ILanguageToken).IsAssignableFrom(c))
                .Select(t => (ILanguageToken)Activator.CreateInstance(t, null));

            var generator = new ParserGenerator(grammar.ToList());

            LanguageParser parser = generator.GetParser();

            var tokens = parser.Parse(source);

            foreach (var token in tokens)
            {
                ((ICodeEmitter)token).Emit(context);
            }
        }

        private static void CreateObjectFile(string outputObjFile, IArchitecture architecture, List<IRInstruction> ir, Dictionary<string, StringConstant> stringConstants, Dictionary<string, Variable> globalVariables, Dictionary<string, Function> functions)
        {
            ShowIR(ir);

            var header = new ObjectFileHeader();

            header.RelocationAddresses = new List<int>();
            header.LabelAddresses = new List<LabelAddressTableEntry>();
            
            var code = new List<byte>();

            int initializedDataSize = 0;

            //name => labelIndex
            header.ExportedSymbols = new Dictionary<string, int>();

            #region String Constants

            if (stringConstants != null)
            {
                foreach (var str in stringConstants)
                {
                    header.LabelAddresses.Add(
                            new LabelAddressTableEntry()
                            {
                                Index = str.Value.LabelAddress,
                                Address = initializedDataSize
                            }
                        );

                    initializedDataSize += str.Value.Value.Length + 1;
                }
            }

            #endregion

            int uninitializedDataSize = 0;

            #region Global Variables

            if (globalVariables != null)
            {
                foreach (var variable in globalVariables)
                {
                    if (variable.Value.IsExported)
                    {
                        header.ExportedSymbols.Add(variable.Key, variable.Value.Address.Value);
                    }

                    if (variable.Value.IsExtern)
                    {
                        header.LabelAddresses.Add(
                            new LabelAddressTableEntry()
                            {
                                Index = variable.Value.Address.Value,
                                IsExtern = true,
                                SymbolName = variable.Key
                            }
                        );
                    }
                    else
                    {
                        header.LabelAddresses.Add(
                            new LabelAddressTableEntry()
                            {
                                Index = variable.Value.Address.Value,
                                IsExtern = false,
                                Address = initializedDataSize + uninitializedDataSize
                            }
                        );

                        uninitializedDataSize += variable.Value.Type.GetSize();
                    }
                }
            }

            #endregion

            #region Functions

            if (functions != null)
            {
                foreach (var function in functions)
                {
                    if (function.Value.IsExported)
                    {
                        header.ExportedSymbols.Add(function.Key, function.Value.Address.Value);
                    }
                    else if (function.Value.IsExtern)
                    {
                        header.LabelAddresses.Add(
                            new LabelAddressTableEntry()
                            {
                                Index = function.Value.Address.Value,
                                IsExtern = true,
                                SymbolName = function.Key
                            }
                        );
                    }
                    else
                    {
                        if (function.Key.Equals("main", StringComparison.CurrentCultureIgnoreCase))
                        {
                            header.HasEntryPoint = true;
                            header.EntryPointFunctionLabel = function.Value.Address.Value;
                        }
                    }
                }
            }

            #endregion

            int codeAddress = initializedDataSize + uninitializedDataSize;

            foreach (var i in ir)
            {
                if (i is IRComment)
                {
                    continue;
                }

                if (i is IRLabel)
                {
                    header.LabelAddresses.Add(
                        new LabelAddressTableEntry() 
                        { 
                            Index = ((IRLabel)i).Index, 
                            Address = codeAddress 
                        }
                    );
                }

                if (i is IRelocatableAddressValue && ((IRelocatableAddressValue)i).HasRelocatableAddressValue())
                {
                    int offset = architecture.GetRelocationOffset(i);
                    header.RelocationAddresses.Add(codeAddress + offset);
                }

                byte[] machineInstruction = i.GetImplementation(architecture);
                code.AddRange(machineInstruction.ToList());
                codeAddress += machineInstruction.Length;
            }

            if (!String.IsNullOrEmpty(outputObjFile))
            {
                using (var sw = new BinaryWriter(new FileStream(outputObjFile, FileMode.Create)))
                {
                    ObjectFileUtils.WriteObjectFileHeader(header, sw);

                    //Initialized data

                    if (stringConstants != null)
                    {
                        foreach (var str in stringConstants)
                        {
                            foreach (var ch in str.Value.Value.ToCharArray())
                            {
                                sw.Write((byte)ch);
                            }

                            sw.Write((byte)0); //0 terminated strings
                        }
                    }

                    //Uninitialized data
                    for (int i = 0; i < uninitializedDataSize; i++)
                    {
                        sw.Write((byte)0);
                    }

                    //Code section
                    foreach (byte b in code)
                    {
                        sw.Write(b);
                    }
                }
            }
        }

        private static void ShowIR(List<IRInstruction> ir)
        {
            foreach (var i in ir)
            {
                Console.WriteLine(i.Display());
            }
        }
    }
}
