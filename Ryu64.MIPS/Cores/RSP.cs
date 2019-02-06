using System;
using System.Threading;

namespace Ryu64.MIPS.Cores
{
    public class RSP
    {
        public static bool RSP_ON   = false;
        public static bool RSP_HALT
        {
            get
            {
                return (R4300.memory.ReadUInt32(0x04040010) & 1) > 0;
            }
        }

        private static uint[] prevCOP0 = new uint[16];

        public static void InterpretOpcode(uint Opcode)
        {
            Buffer.BlockCopy(Registers.RSPCOP0.Reg, 0, prevCOP0, 0, Registers.RSPCOP0.Reg.Length);

            OpcodeTable.OpcodeDesc Desc = new OpcodeTable.OpcodeDesc(Opcode, true, false);
            OpcodeTable.InstInfo   Info = OpcodeTable.GetOpcodeInfo (Opcode, true, false);

            Registers.RSPCOP0.Reg[0]  = R4300.memory.ReadUInt32(0x04040000);
            Registers.RSPCOP0.Reg[1]  = R4300.memory.ReadUInt32(0x04040004);
            Registers.RSPCOP0.Reg[2]  = R4300.memory.ReadUInt32(0x04040008);
            Registers.RSPCOP0.Reg[3]  = R4300.memory.ReadUInt32(0x0404000C);
            Registers.RSPCOP0.Reg[4]  = R4300.memory.ReadUInt32(0x04040010);
            Registers.RSPCOP0.Reg[8]  = R4300.memory.ReadUInt32(0x04100000);
            Registers.RSPCOP0.Reg[9]  = R4300.memory.ReadUInt32(0x04100004);
            Registers.RSPCOP0.Reg[10] = R4300.memory.ReadUInt32(0x04100008);
            Registers.RSPCOP0.Reg[11] = R4300.memory.ReadUInt32(0x0410000C);

            Info.Interpret(Desc);

            if (Registers.RSPCOP0.Reg[0]  != prevCOP0[0])  R4300.memory.WriteUInt32(0x04040000, Registers.RSPCOP0.Reg[0]);
            if (Registers.RSPCOP0.Reg[1]  != prevCOP0[1])  R4300.memory.WriteUInt32(0x04040004, Registers.RSPCOP0.Reg[1]);
            if (Registers.RSPCOP0.Reg[2]  != prevCOP0[2])  R4300.memory.WriteUInt32(0x04040008, Registers.RSPCOP0.Reg[2]);
            if (Registers.RSPCOP0.Reg[3]  != prevCOP0[3])  R4300.memory.WriteUInt32(0x0404000C, Registers.RSPCOP0.Reg[3]);
            if (Registers.RSPCOP0.Reg[4]  != prevCOP0[4])  R4300.memory.WriteUInt32(0x04040010, Registers.RSPCOP0.Reg[4]);
            if (Registers.RSPCOP0.Reg[8]  != prevCOP0[8])  R4300.memory.WriteUInt32(0x04100000, Registers.RSPCOP0.Reg[8]);
            if (Registers.RSPCOP0.Reg[9]  != prevCOP0[9])  R4300.memory.WriteUInt32(0x04100004, Registers.RSPCOP0.Reg[9]);
            if (Registers.RSPCOP0.Reg[11] != prevCOP0[11]) R4300.memory.WriteUInt32(0x0410000C, Registers.RSPCOP0.Reg[11]);
        }

        public static void PowerOnRSP()
        {
            RSP_ON = true;
            Thread RSPThread = new Thread(() =>
            {
                while (RSP_ON)
                {
                    if (!RSP_HALT)
                    {
                        uint Opcode = R4300.memory.ReadUInt32(Registers.RSPReg.PC, true, false);
                        InterpretOpcode(Opcode);
                    }
                    else
                        Thread.Sleep(2);
                }
            })
            {
                Name = "RSP"
            };
            RSPThread.Start();
        }
    }
}
