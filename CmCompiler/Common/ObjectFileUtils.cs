using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Common
{
    public static class ObjectFileUtils
    {
        public static void WriteObjectFileHeader(ObjectFileHeader header, BinaryWriter bw)
        {
            //Label address table
            byte[] labelAddressTableData;
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                if (header.LabelAddresses.Count > 0)
                {
                    for (int i = 0; i < header.LabelAddresses.Max(a => a.Index) + 1; i++)
                    {
                        var addressRow = header.LabelAddresses.FirstOrDefault(a => a.Index == i);

                        if (addressRow == null)
                        {
                            stream.Write(0); //type
                            stream.Write(0); //value
                        }
                        else
                        {
                            if (addressRow.IsExtern)
                            {
                                stream.Write(2); //type
                                stream.Write(addressRow.SymbolName);
                            }
                            else
                            {
                                stream.Write(1);
                                stream.Write(addressRow.Address);
                            }
                        }
                    }
                }

                labelAddressTableData = ((MemoryStream)stream.BaseStream).ToArray();
            }

            //Symbol table
            byte[] symbolTableData;
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                foreach (var exportedSymbol in header.ExportedSymbols)
                {
                    stream.Write(exportedSymbol.Key);
                    stream.Write(exportedSymbol.Value);
                }

                symbolTableData = ((MemoryStream)stream.BaseStream).ToArray();
            }

            //Compute offsets
            int relocationsOffset = 24;
            int labelsOffset = relocationsOffset + (header.RelocationAddresses.Count * 4);
            int symbolTableOffset = labelsOffset + labelAddressTableData.Length;
            int dataOffset = symbolTableOffset + symbolTableData.Length;

            //Offset values
            bw.Write(header.HasEntryPoint ? 1 : 0);
            bw.Write(header.EntryPointFunctionLabel);
            bw.Write(header.RelocationAddresses.Count);             //# relocations
            if (header.LabelAddresses.Count > 0)
            {
                bw.Write(header.LabelAddresses.Max(a => a.Index) + 1);  //# labels
            }
            else
            {
                bw.Write(0);
            }
            bw.Write(header.ExportedSymbols.Count);                 //# symbols
            bw.Write(dataOffset);                                   //where data section starts

            //Relocation addresses
            foreach (int loc in header.RelocationAddresses)
            {
                bw.Write(loc);
            }

            //Label addresses
            bw.Write(labelAddressTableData, 0, labelAddressTableData.Length);

            //Symbol table
            bw.Write(symbolTableData, 0, symbolTableData.Length);
        }

        public static ObjectFileHeader ReadObjectFileHeader(string filePath)
        {
            var header = new ObjectFileHeader();

            using (var br = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                int hasEntryPoint = br.ReadInt32();
                int entryPointFunctionLabel = br.ReadInt32();
                int numRelocations = br.ReadInt32();
                int numLabels = br.ReadInt32();
                int numSymbols = br.ReadInt32();
                int dataOffset = br.ReadInt32();

                header.DataStart = dataOffset;
                header.SizeOfDataAndCode = (int)br.BaseStream.Length - dataOffset;

                header.HasEntryPoint = hasEntryPoint == 1;
                header.EntryPointFunctionLabel = entryPointFunctionLabel;

                //Read relocations
                header.RelocationAddresses = new List<int>();

                for (int i = 0; i < numRelocations; i++)
                {
                    header.RelocationAddresses.Add(br.ReadInt32());
                }

                //Read label addresses
                header.LabelAddresses = new List<LabelAddressTableEntry>();

                for (int i = 0; i < numLabels; i++)
                {
                    int type = br.ReadInt32();

                    switch (type)
                    {
                        case 0: //null entry
                            header.LabelAddresses.Add(null);
                            break;
                        case 1: //internal address
                            header.LabelAddresses.Add(
                                new LabelAddressTableEntry()
                                {
                                    Index = i,
                                    IsExtern = false,
                                    Address = br.ReadInt32()
                                }
                            );
                            break;
                        case 2: //external symbol
                            header.LabelAddresses.Add(
                                new LabelAddressTableEntry()
                                {
                                    Index = i,
                                    IsExtern = true,
                                    SymbolName = br.ReadString()
                                }
                            );
                            break;
                    }
                }

                //Read exported symbols
                header.ExportedSymbols = new Dictionary<string, int>();

                for (int i = 0; i < numSymbols; i++)
                {
                    string name = br.ReadString();
                    int labelNum = br.ReadInt32();

                    header.ExportedSymbols.Add(name, labelNum);
                }
            }

            return header;
        }
    }
}
