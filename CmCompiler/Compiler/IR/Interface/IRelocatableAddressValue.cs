using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Context;

namespace CmC.Compiler.IR.Interface
{
    public interface IRelocatableAddressValue
    {
        bool HasRelocatableAddressValue();
        int GetLabelIndex();
    }
}
