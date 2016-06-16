using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Common;
using CmC.Compiler.Exceptions;

namespace CmC.Linker
{
    public static class CmLinker
    {
        public static void Link(Stream toStream, List<string> objectFiles, bool createExecutable, int loadAddress = 0)
        {
            List<byte[]> objectFileCodes = new List<byte[]>();

            foreach (var filePath in objectFiles)
            {
                using (var fs = new FileStream(filePath, FileMode.Open))
                using (var s = new MemoryStream())
                {
                    fs.CopyTo(s);
                    objectFileCodes.Add(s.GetBuffer());
                }
            }

            Link(toStream, objectFileCodes, createExecutable, loadAddress);
        }

        public static void Link(Stream toStream, List<byte[]> objectFileCodes, bool createExecutable, int loadAddress = 0)
        {
            var headers = new List<ObjectCodeHeader>();
            var objDataOffsets = new List<int>();

            int objDataOffset = 0;

            bool foundEntryPoint = false;
            
            for(int i = 0; i < objectFileCodes.Count; i++)
            {
                objDataOffsets.Add(objDataOffset + ((createExecutable && i == 0) ? 4 : 0));

                var header = ObjectCodeUtils.ReadObjectCodeHeader(objectFileCodes[i]);

                headers.Add(header);

                if (header.HasEntryPoint)
                {
                    if (foundEntryPoint)
                    {
                        throw new Exception("Multiple entry points detected in source object files");
                    }
                    else
                    {
                        foundEntryPoint = true;
                    }
                }

                objDataOffset += header.SizeOfDataAndCode;
            }

            if (createExecutable && !foundEntryPoint)
            {
                throw new Exception("No entry point found in any source object files");
            }

            using (var stream = new MemoryStream())
            using (var sw = new BinaryWriter(stream))
            using (var sr = new BinaryReader(stream))
            {
                if (createExecutable)
                {
                    loadAddress += 4;
                    sw.Write(0); //temporary until entry point address is resolved
                }

                for (int i = 0; i < objectFileCodes.Count; i++)
                {
                    //Write data and code to output file
                    using (var objStream = new BinaryReader(new MemoryStream(objectFileCodes[i])))
                    {
                        objStream.BaseStream.Seek(headers[i].DataStart, SeekOrigin.Begin);

                        byte[] dataAndCode = new byte[objStream.BaseStream.Length - headers[i].DataStart];
                        objStream.Read(dataAndCode, 0, dataAndCode.Length);

                        sw.Seek(objDataOffsets[i], SeekOrigin.Begin);
                        sw.Write(dataAndCode);
                    }

                    //Perform relocations and symbol resolution
                    foreach(int address in headers[i].RelocationAddresses)
                    {
                        sr.BaseStream.Seek(address + objDataOffsets[i], SeekOrigin.Begin);

                        int labelIndex = BitConverter.ToInt32(BitConverter.GetBytes(sr.ReadInt32()).Reverse().ToArray(), 0);
                        var label = headers[i].LabelAddresses[labelIndex];
                        int newAddress = -1;

                        if (label.IsExtern)
                        {
                            bool resolved = false;

                            //search all other header symbol tables
                            for (int j = 0; j < objectFileCodes.Count; j++)
                            {
                                if (i == j) continue;

                                if (headers[j].ExportedSymbols.ContainsKey(label.SymbolName))
                                {
                                    resolved = true;
                                    int extLabelIndex = headers[j].ExportedSymbols[label.SymbolName];
                                    newAddress = headers[j].LabelAddresses[extLabelIndex].Address;
                                    newAddress += objDataOffsets[j] + loadAddress;
                                }
                            }

                            if (!resolved)
                            {
                                throw new UnresolvedExternalSymbolException(label.SymbolName);
                            }
                        }
                        else
                        {
                            //Write new address
                            newAddress = label.Address + objDataOffsets[i] + loadAddress;

                            if (createExecutable)
                            {
                                if (headers[i].HasEntryPoint && headers[i].EntryPointFunctionLabel == labelIndex)
                                {
                                    //Write entry point address at position 0
                                    sw.Seek(0, SeekOrigin.Begin);
                                    sw.Write(BitConverter.GetBytes(newAddress).Reverse().ToArray());
                                }
                            }
                        }

                        sw.Seek(address + objDataOffsets[i], SeekOrigin.Begin);
                        sw.Write(BitConverter.GetBytes(newAddress).Reverse().ToArray());
                    }
                }

                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(toStream);
            }
        }
    }
}
