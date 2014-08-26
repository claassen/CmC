using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmC.Context
{
    public class Op
    {
        public string Name;
        public string R1;
        public string R2;
        public string R3;
        public ImmediateValue Imm;

        public override string ToString()
        {
            return Name + " " +
                (R1 != null ? R1 + ", " : "") +
                (R2 != null ? R2 + ", " : "") +
                (R3 != null ? R3 + ", " : "") +
                (Imm != null ? "$" + Imm : "");
        }
    }
}
