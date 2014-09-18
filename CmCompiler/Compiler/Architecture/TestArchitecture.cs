using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.Architecture
{
    public class TestArchitecture : IArchitecture
    {
        private int count;

        public int GetRelocationOffset(IRInstruction ir)
        {
            return 0;
        }

        public byte[] Implement(IRAdd ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRAnd ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRCall ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRCompareImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Right.Value)
            );
        }

        public byte[] Implement(IRCompareRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRDiv ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRJumpEQ ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpGE ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpGT ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpLE ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpLT ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpNE ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRJumpRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRLoadImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Value)
            );
        }

        public byte[] Implement(IRLoadRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRLoadRegisterPlusImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Offset.Value)
            );
        }

        public byte[] Implement(IRMoveImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Value.Value)
            );
        }

        public byte[] Implement(IRMoveRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRMult ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRNoop ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IROr ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRPop ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRPushImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Value.Value)
            );
        }

        public byte[] Implement(IRPushRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRShiftLeft ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRShiftRight ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRStoreImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.To.Value)
            );
        }

        public byte[] Implement(IRStoreRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRStoreRegisterPlusImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Offset.Value)
            );
        }

        public byte[] Implement(IRSub ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRXOr ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRMemCopy ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Length.Value)
            );
        }

        public byte[] Implement(IRHalt ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRInt ir)
        {
            throw new NotImplementedException();
        }

        public byte[] Implement(IRIRet ir)
        {
            throw new NotImplementedException();
        }


        public byte[] Implement(IRRet ir)
        {
            return BitConverter.GetBytes(count++);
        }


        public byte[] Implement(IRArchitectureSpecificAsm ir)
        {
            throw new NotImplementedException();
        }


        public byte[] Implement(IRSetIDT ir)
        {
            throw new NotImplementedException();
        }

        public byte[] Implement(IRSetPT ir)
        {
            throw new NotImplementedException();
        }
    }
}
