using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmC.Compiler.IR;
using CmC.Compiler.IR.Interface;

namespace CmC.Compiler.Architecture
{
    public enum OpCode : byte
    {
        HALT = 0,
        ADDR,
        ANDR,
        CALL,
        CMPI,
        CMPR,
        DIVR,
        JEQI,
        JGEI,
        JGTI,
        JMPI,
        JLEI,
        JLTI,
        JNEI,
        JMPR,
        LOADI,
        LOADR,
        LOADIR,
        MEMCPY,
        MOVI,
        MOVR,
        MULTR,
        NOOP,
        ORR,
        POPR,
        PUSHI,
        PUSHR,
        SHLR,
        SHRR,
        STOREI,
        STORER,
        STOREIR,
        SUBR,
        XORR,

        SYSENT,
        SYSEX,
        FSREAD,
        FSWRITE,
        SETIDT
    }

    public class VMArchitecture : IArchitecture
    {
        private byte[] Encode(OpCode opCode, int r1, int r2, int r3, int operandSize, bool hasImmediate, int immediate = 0)
        {
            int instr = ((int)opCode << 24) |
                        ((hasImmediate ? 1 : 0) << 20 |
                        ((int)r1 << 16) |
                        ((int)r2 << 12) |
                        ((int)r3 << 8) |
                         (byte)operandSize);

            byte[] bytes = BitConverter.GetBytes(instr).Reverse().ToArray();

            if (!hasImmediate)
            {
                return bytes;
            }
            else
            {
                return bytes.Concat(BitConverter.GetBytes(immediate).Reverse()).ToArray();
            }
        }

        private int GetRegisterIndex(string reg)
        {
            switch(reg)
            {
                case "eax":
                    return 0;
                case "ebx":
                    return 1;
                case "ecx":
                    return 2;
                case "sp":
                    return 4;
                case "bp":
                    return 5;
                default:
                    throw new Exception("Unknown register name");
            }
        }

        public int GetRelocationOffset(IRInstruction ir)
        {
            return 4;
        }

        public byte[] Implement(IRAdd ir)
        {
            return Encode(OpCode.ADDR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRAnd ir)
        {
            return Encode(OpCode.ANDR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRCall ir)
        {
            throw new NotImplementedException();
        }

        public byte[] Implement(IRCompareImmediate ir)
        {
            return Encode(OpCode.CMPI, GetRegisterIndex(ir.Left), 0, 0, ir.OperandBytes, true, ir.Right.Value);
        }

        public byte[] Implement(IRCompareRegister ir)
        {
            return Encode(OpCode.CMPR, GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRDiv ir)
        {
            return Encode(OpCode.DIVR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRJumpEQ ir)
        {
            return Encode(OpCode.JEQI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpGE ir)
        {
            return Encode(OpCode.JGEI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpGT ir)
        {
            return Encode(OpCode.JGTI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpImmediate ir)
        {
            return Encode(OpCode.JMPI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpLE ir)
        {
            return Encode(OpCode.JLEI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpLT ir)
        {
            return Encode(OpCode.JLTI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpNE ir)
        {
            return Encode(OpCode.JNEI, 0, 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRJumpRegister ir)
        {
            return Encode(OpCode.JMPR, GetRegisterIndex(ir.Address), 0, 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRLoadImmediate ir)
        {
            return Encode(OpCode.LOADI, GetRegisterIndex(ir.To), 0, 0, ir.OperandBytes, true, ir.Address.Value);
        }

        public byte[] Implement(IRLoadRegister ir)
        {
            return Encode(OpCode.LOADR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.From), 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRLoadRegisterPlusImmediate ir)
        {
            return Encode(OpCode.LOADIR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.From), 0, ir.OperandBytes, true, ir.Offset.Value);
        }

        public byte[] Implement(IRMoveImmediate ir)
        {
            return Encode(OpCode.MOVI, GetRegisterIndex(ir.To), 0, 0, ir.OperandBytes, true, ir.Value.Value);
        }

        public byte[] Implement(IRMoveRegister ir)
        {
            return Encode(OpCode.MOVR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.From), 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRMult ir)
        {
            return Encode(OpCode.MULTR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRNoop ir)
        {
            return Encode(OpCode.NOOP, 0, 0, 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IROr ir)
        {
            return Encode(OpCode.ORR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRPop ir)
        {
            return Encode(OpCode.POPR, GetRegisterIndex(ir.To), 0, 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRPushImmediate ir)
        {
            return Encode(OpCode.PUSHI, 0, 0, 0, ir.OperandBytes, true, ir.Value.Value);
        }

        public byte[] Implement(IRPushRegister ir)
        {
            return Encode(OpCode.PUSHR, GetRegisterIndex(ir.From), 0, 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRShiftLeft ir)
        {
            throw new NotImplementedException();
        }

        public byte[] Implement(IRShiftRight ir)
        {
            throw new NotImplementedException();
        }

        public byte[] Implement(IRStoreImmediate ir)
        {
            return Encode(OpCode.STOREI, GetRegisterIndex(ir.From), 0, 0, ir.OperandBytes, true, ir.To.Value);
        }

        public byte[] Implement(IRStoreRegister ir)
        {
            return Encode(OpCode.STORER, GetRegisterIndex(ir.To), GetRegisterIndex(ir.From), 0, ir.OperandBytes, false);
        }

        public byte[] Implement(IRStoreRegisterPlusImmediate ir)
        {
            return Encode(OpCode.STOREIR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.From), 0, ir.OperandBytes, true, ir.Offset.Value);
        }

        public byte[] Implement(IRSub ir)
        {
            return Encode(OpCode.SUBR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRXOr ir)
        {
            return Encode(OpCode.XORR, GetRegisterIndex(ir.To), GetRegisterIndex(ir.Left), GetRegisterIndex(ir.Right), ir.OperandBytes, false);
        }

        public byte[] Implement(IRMemCopy ir)
        {
            return Encode(OpCode.MEMCPY, GetRegisterIndex(ir.To), GetRegisterIndex(ir.From), 0, ir.OperandBytes, true, ir.Length.Value);
        }

        public byte[] Implement(IRHalt ir)
        {
            return Encode(OpCode.HALT, 0, 0, 0, 0, false);
        }
    }
}
