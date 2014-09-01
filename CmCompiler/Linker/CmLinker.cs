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
        public static void Link(List<string> objectFiles, string outputFile)
        {
            var headers = new List<ObjectFileHeader>();
            var objDataOffsets = new List<int>();

            int objDataOffset = 0;

            foreach (string objFilePath in objectFiles)
            {
                objDataOffsets.Add(objDataOffset);

                var header = ObjectFileUtils.ReadObjectFileHeader(objFilePath);

                headers.Add(header);

                objDataOffset += header.SizeOfDataAndCode;
            }

            using (var stream = new FileStream(outputFile, FileMode.Create))
            using (var sw = new BinaryWriter(stream))
            using (var sr = new BinaryReader(stream))
            {
                for (int i = 0; i < objectFiles.Count; i++)
                {
                    //Write data and code to output file
                    using (var objStream = new BinaryReader(new FileStream(objectFiles[i], FileMode.Open)))
                    {
                        objStream.BaseStream.Seek(headers[i].DataStart, SeekOrigin.Begin);

                        byte[] dataAndCode = new byte[objStream.BaseStream.Length - headers[i].DataStart];
                        objStream.Read(dataAndCode, 0, dataAndCode.Length);

                        sw.BaseStream.Seek(objDataOffsets[i], SeekOrigin.Begin);
                        sw.Write(dataAndCode);
                    }

                    //Perform relocations and symbol resolution
                    foreach(int address in headers[i].RelocationAddresses)
                    {
                        sr.BaseStream.Seek(address + objDataOffsets[i], SeekOrigin.Begin);

                        int labelIndex = sr.ReadInt32();
                        var label = headers[i].LabelAddresses[labelIndex];
                        int newAddress = -1;

                        if (label.IsExtern)
                        {
                            bool resolved = false;

                            //search all other header symbol tables
                            for (int j = 0; j < objectFiles.Count; j++)
                            {
                                if (i == j) continue;

                                if (headers[j].ExportedSymbols.ContainsKey(label.SymbolName))
                                {
                                    resolved = true;
                                    int extLabelIndex = headers[j].ExportedSymbols[label.SymbolName];
                                    newAddress = headers[j].LabelAddresses[extLabelIndex].Address;
                                    newAddress += objDataOffsets[j];
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
                            newAddress = label.Address + objDataOffsets[i];
                        }

                        sw.BaseStream.Seek(address + objDataOffsets[i], SeekOrigin.Begin);
                        sw.Write(newAddress);
                    }
                }
            }
        }
    }
}
