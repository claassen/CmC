using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;
using CmC.Compiler.Tokens.TokenInterfaces;
using ParserGen.Generator;
using ParserGen.Parser;
using ParserGen.Parser.Tokens;

namespace CmC.Compiler
{
    public class CmCompiler
    {
        public byte[] Compile(string source)
        {
            return Compile(source, new TestArchitecture());
        }

        public byte[] Compile(string source, IArchitecture architecture)
        {
            var grammar = Assembly.GetExecutingAssembly().GetTypes()
                .Where(c => c.Namespace == "CmC.Compiler.Tokens")
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

            return CreateObjectCode(context, architecture);
        }

        private byte[] CreateObjectCode(CompilationContext context, IArchitecture architecture)
        {
            var ir = context.GetIR();

            int byteCount = 0;

            var labelAddresses = new List<Tuple<int, int>>();
            var relocationAddresses = new List<int>();
            var code = new List<byte>();

            int globalDataSize = 0;

            foreach (var variable in context.GetGlobalVariables())
            {
                labelAddresses.Add(new Tuple<int,int>(variable.Address.Number, globalDataSize));
                globalDataSize += variable.Type.GetSize();
            }

            byteCount = globalDataSize;

            foreach (var i in ir)
            {
                if (i is IRComment)
                {
                    continue;
                }

                if (i is IRLabel)
                {
                    labelAddresses.Add(new Tuple<int, int>(((IRLabel)i).Index, byteCount));
                }

                if (i is IRelocatableAddressValue)
                {
                    int offset = architecture.GetRelocationOffset(i);
                    relocationAddresses.Add(byteCount + offset);
                }

                byte[] machineInstruction = i.GetImplementation(architecture);
                code.AddRange(machineInstruction.ToList());
                byteCount += machineInstruction.Length;
            }

            byte[] fileData;

            int dataOffset = 8 + (relocationAddresses.Count * 4) + (labelAddresses.Count * 4);

            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                stream.Write(dataOffset);
                stream.Write(relocationAddresses.Count);

                foreach (int loc in relocationAddresses)
                {
                    stream.Write(loc);
                }

                for(int i = 0; i < labelAddresses.Max(a => a.Item1); i++)
                {
                    var address = labelAddresses.FirstOrDefault(a => a.Item1 == i);

                    if(address != null)
                    {
                        stream.Write(address.Item2);
                    }
                    else
                    {
                        stream.Write(0);
                    }
                }

                for (int i = 0; i < globalDataSize; i++)
                {
                    //Zero global/static data memory
                    stream.Write(0);
                }

                foreach (byte b in code)
                {
                    stream.Write(b);
                }

                fileData = ((MemoryStream)stream.BaseStream).ToArray();
            }

            return fileData;
        }
    }
}
