﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Common
{
    public class ObjectCodeHeader
    {
        public int DataStart;
        public int SizeOfDataAndCode;
        public bool HasEntryPoint;
        public int EntryPointFunctionLabel;

        public List<int> RelocationAddresses;
        public List<LabelAddressTableEntry> LabelAddresses;
        public Dictionary<string, int> ExportedSymbols;
    }
}
