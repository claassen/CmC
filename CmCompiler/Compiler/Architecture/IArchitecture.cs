using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.Architecture
{
    public interface IArchitecture
    {
        int GetRelocationOffset(IRInstruction ir);

        byte[] Implement(IRAdd ir);
        byte[] Implement(IRAnd ir);
        byte[] Implement(IRCall ir);
        byte[] Implement(IRRet ir);
        byte[] Implement(IRCompareImmediate ir);
        byte[] Implement(IRCompareRegister ir);
        byte[] Implement(IRDiv ir);
        byte[] Implement(IRJumpEQ ir);
        byte[] Implement(IRJumpGE ir);
        byte[] Implement(IRJumpGT ir);
        byte[] Implement(IRJumpImmediate ir);
        byte[] Implement(IRJumpLE ir);
        byte[] Implement(IRJumpLT ir);
        byte[] Implement(IRJumpNE ir);
        byte[] Implement(IRJumpRegister ir);
        byte[] Implement(IRLoadImmediate ir);
        byte[] Implement(IRLoadRegister ir);
        byte[] Implement(IRLoadRegisterPlusImmediate ir);
        byte[] Implement(IRMoveImmediate ir);
        byte[] Implement(IRMoveRegister ir);
        byte[] Implement(IRMult ir);
        byte[] Implement(IRNoop ir);
        byte[] Implement(IROr ir);
        byte[] Implement(IRPop ir);
        byte[] Implement(IRPushImmediate ir);
        byte[] Implement(IRPushRegister ir);
        byte[] Implement(IRShiftLeft ir);
        byte[] Implement(IRShiftRight ir);
        byte[] Implement(IRStoreImmediate ir);
        byte[] Implement(IRStoreRegister ir);
        byte[] Implement(IRStoreRegisterPlusImmediate ir);
        byte[] Implement(IRSub ir);
        byte[] Implement(IRXOr ir);
        byte[] Implement(IRMemCopy ir);
        byte[] Implement(IRHalt ir);
        byte[] Implement(IRSysEnt ir);
        byte[] Implement(IRSysEx ir);
    }
}
