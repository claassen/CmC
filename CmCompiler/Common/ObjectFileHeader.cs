using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Common
{
    public class ObjectFileHeader
    {
        public int DataStart;
        public int SizeOfDataAndCode;

        public List<int> RelocationAddresses;
        public List<LabelAddressTableEntry> LabelAddresses;
        public Dictionary<string, int> ExportedSymbols;
    }
}
