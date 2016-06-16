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
        public static CompilationContext CompileFile(string path)
        {
            using (var stream = new StreamReader(new FileStream(path, FileMode.Open)))
            {
                return CompileText(stream.ReadToEnd(), Path.GetDirectoryName(path));
            }
        }

        public static CompilationContext CompileText(string text, string sourceFolder = "")
        {
            if (!String.IsNullOrEmpty(sourceFolder))
            {
                CompilerUtils.ProcessIncludes(ref text, sourceFolder);
            }

            CompilerUtils.RemoveComments(ref text);
            CompilerUtils.ProcessMacros(ref text);

            var context = new CompilationContext();

            ProcessSourceText(text, context);

            return context;
        }

        private static void ProcessSourceText(string text, CompilationContext context)
        {
            var grammar = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.Namespace == "CmC.Compiler.Syntax" || c.Namespace == "CmC.Compiler.Syntax.Common")
                .Where(c => typeof(ILanguageToken).IsAssignableFrom(c))
                .Select(t => (ILanguageToken)Activator.CreateInstance(t, null));

            var generator = new ParserGenerator(grammar.ToList());

            LanguageParser parser = generator.GetParser();

            var tokens = parser.Parse(text);

            foreach (var token in tokens)
            {
                ((ICodeEmitter)token).Emit(context);
            }            
        }

        public static void CreateObjectCode(Stream toStream, IArchitecture architecture, List<IRInstruction> ir, Dictionary<string, StringConstant> stringConstants, Dictionary<string, Variable> globalVariables, Dictionary<string, Function> functions)
        {
            var header = new ObjectCodeHeader();

            header.RelocationAddresses = new List<int>();
            header.LabelAddresses = new List<LabelAddressTableEntry>();
            
            var code = new List<byte>();

            //name => labelIndex
            header.ExportedSymbols = new Dictionary<string, int>();

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

            int codeAddress = 0; 

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
                    int relocOffset = architecture.GetRelocationOffset(i);
                    header.RelocationAddresses.Add(codeAddress + relocOffset);
                }

                byte[] machineInstruction = i.GetImplementation(architecture);
                code.AddRange(machineInstruction.ToList());
                codeAddress += machineInstruction.Length;
            }

            int offset = codeAddress;
            int initializedDataSize = 0;

            #region String Constants

            if (stringConstants != null)
            {
                foreach (var str in stringConstants)
                {
                    header.LabelAddresses.Add(
                        new LabelAddressTableEntry()
                        {
                            Index = str.Value.LabelAddress,
                            Address = offset + initializedDataSize
                        }
                    );

                    initializedDataSize += str.Value.Value.Length + 1;
                }
            }

            #endregion

            offset += initializedDataSize;
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
                                Address = offset + uninitializedDataSize
                            }
                        );

                        uninitializedDataSize += variable.Value.Type.GetSize();
                    }
                }
            }

            #endregion

            offset += uninitializedDataSize;

            header.SizeOfDataAndCode = offset;

            using (var sw = new BinaryWriter(toStream))
            {
                ObjectCodeUtils.WriteObjectFileHeader(header, sw);

                //Code section
                foreach (byte b in code)
                {
                    sw.Write(b);
                }

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
            }
        }

        public static void GenerateAssemblyOutput(Stream stream, List<IRInstruction> ir, bool includeComments)
        {
            using(var sw = new StreamWriter(stream))
            {
                foreach (var i in ir)
                {
                    if (includeComments || !(i is IRComment))
                    {
                        sw.WriteLine(i.Display());
                    }
                }
            }
        }
    }
}
