using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.Architecture;
using CmC.Compiler.Context;

namespace CmC.Compiler.IR.Interface
{
    public interface IRInstruction
    {
        byte[] GetImplementation(IArchitecture arch);
        string Display();
    }
}
