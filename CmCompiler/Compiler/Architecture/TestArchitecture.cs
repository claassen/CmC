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
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRCompareImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Right.Number)
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
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpGE ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpGT ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpLE ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpLT ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpNE ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRJumpRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRLoadImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Address.Number)
            );
        }

        public byte[] Implement(IRLoadRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRLoadRegisterPlusImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Offset.Number)
            );
        }

        public byte[] Implement(IRMoveImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Value.Number)
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
                (long)((long)(count++) << 32 | (uint)ir.Value.Number)
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
                (long)((long)(count++) << 32 | (uint)ir.To.Number)
            );
        }

        public byte[] Implement(IRStoreRegister ir)
        {
            return BitConverter.GetBytes(count++);
        }

        public byte[] Implement(IRStoreRegisterPlusImmediate ir)
        {
            return BitConverter.GetBytes(
                (long)((long)(count++) << 32 | (uint)ir.Offset.Number)
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
                (long)((long)(count++) << 32 | (uint)ir.Length.Number)
            );
        }
    }
}
