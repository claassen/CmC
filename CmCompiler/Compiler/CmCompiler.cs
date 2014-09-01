using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public static void Compile(string source, string outputObjFile = "")
        {
            Compile(source, outputObjFile, new TestArchitecture());
        }

        public static void Compile(string source, string outputObjFile, IArchitecture architecture)
        {
            var grammar = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.Namespace == "CmC.Compiler.Syntax")
                .Where(c => typeof(ILanguageToken).IsAssignableFrom(c))
                .Select(t => (ILanguageToken)Activator.CreateInstance(t, null));

            var generator = new ParserGenerator(grammar.ToList());
            
            LanguageParser parser = generator.GetParser();

            var tokens = parser.Parse(source);

            var context = new CompilationContext();

            foreach (var token in tokens)
            {
                ((ICodeEmitter)token).Emit(context);
            }

            CreateObjectFile(context, outputObjFile, architecture);
        }

        /*
         * Object file format:
         *  (int)                 # relocations
         *  (int)                 # label addresses
         *  (int)                 # exported symbols
         *  (int)                 .data section offset
         *  (int)[]               relocations - addresses relative to start of .data section
         *  (byte,(int|string))[] label addresses - type followed by either address relative to start of .data section or symbol name
         *  (string,int)[]        exported symbols - symbol name followed by index into label addresses
         */
        private static void CreateObjectFile(CompilationContext context, string outputObjFile, IArchitecture architecture)
        {
            var ir = context.GetIR();

            ShowIR(ir);

            var header = new ObjectFileHeader();

            header.RelocationAddresses = new List<int>();
            header.LabelAddresses = new List<LabelAddressTableEntry>();
            
            var code = new List<byte>();

            int globalDataSize = 0;

            //name => labelIndex
            header.ExportedSymbols = new Dictionary<string, int>();

            foreach (var variable in context.GetGlobalVariables())
            {
                if (variable.Value.IsExported)
                {
                    //name => labelIndex
                    header.ExportedSymbols.Add(variable.Key, variable.Value.Address.Number);
                }
                
                if (variable.Value.IsExtern)
                {
                    header.LabelAddresses.Add(
                        new LabelAddressTableEntry() 
                        { 
                            Index = variable.Value.Address.Number, 
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
                            Index = variable.Value.Address.Number, 
                            IsExtern = false, 
                            Address = globalDataSize 
                        }
                    );

                    globalDataSize += variable.Value.Type.GetSize();
                }
            }

            foreach (var function in context.GetFunctions())
            {
                if (function.Value.IsExported)
                {
                    header.ExportedSymbols.Add(function.Key, function.Value.Address.Number);
                }
                else if (function.Value.IsExtern)
                {
                    //Need to add label address table entry that indicates function addresses is resolved externally
                    header.LabelAddresses.Add(
                        new LabelAddressTableEntry() 
                        { 
                            Index = function.Value.Address.Number, 
                            IsExtern = true, 
                            SymbolName = function.Key 
                        }
                    );
                }
            }

            int codeAddress = globalDataSize;

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

                    //Data section
                    for (int i = 0; i < globalDataSize; i++)
                    {
                        //Zero global/static data memory
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
