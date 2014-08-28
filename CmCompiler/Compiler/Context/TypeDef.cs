using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Compiler.Context
{
    public class TypeDef
    {
        public string Name;
        public virtual int Size { get; set; }
    }
}
