using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class OpcodeTable
    {
        struct InstInfo
        {
            public uint Mask;
            public uint Value;

            public InstInterp.InterpretOpcode Interpret;

            public InstInfo(uint Mask, uint Value, InstInterp.InterpretOpcode Interpret)
            {
                this.Mask      = Mask;
                this.Value     = Value;
                this.Interpret = Interpret;
            }
        }

        public struct OpcodeDesc
        {
            public uint Opcode;

            public byte  op1;
            public byte  op2;
            public byte  op3;
            public byte  op4;
            public short Imm;
            public uint  Target;
            public uint  ExceptionCode20bit;
            public uint  ExceptionCode10bit;

            public OpcodeDesc(uint Opcode)
            {
                this.Opcode = Opcode;

                op1 = (byte)        ((Opcode & 0b00000011111000000000000000000000) >> 19);
                op2 = (byte)        ((Opcode & 0b00000000000111110000000000000000) >> 14);
                op3 = (byte)        ((Opcode & 0b00000000000000001111100000000000) >> 9);
                op4 = (byte)        ((Opcode & 0b00000000000000000000011111000000) >> 4);
                Imm = (short)       ((Opcode & 0b00000000000000001111111111111111));
                Target =             (Opcode & 0b00000011111111111111111111111111);
                ExceptionCode20bit = (Opcode & 0b00000011111111111111111111000000) >> 4;
                ExceptionCode10bit = (Opcode & 0b00000000000000001111111111000000) >> 4;
            }
        }

        private static List<InstInfo> AllInsts;

        public static void Init()
        {
            AllInsts = new List<InstInfo>();

            // Load / Store Instructions
            SetOpcode("100000XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LB);
            SetOpcode("100100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LBU);
            SetOpcode("110111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LD);
            SetOpcode("011010XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LDL);
            SetOpcode("011011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LDR);
            SetOpcode("100001XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LH);
            SetOpcode("100101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LHU);
            SetOpcode("100011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LW);
            SetOpcode("100010XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LWL);
            SetOpcode("100110XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LWR);
            SetOpcode("100111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LWU);
            SetOpcode("101000XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SB);
            SetOpcode("111111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SD);
            SetOpcode("101100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SDL);
            SetOpcode("101101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SDR);
        }

        public static InstInterp.InterpretOpcode GetInterpreterMethod(uint Opcode)
        {
            foreach (InstInfo info in AllInsts)
                if ((Opcode & info.Mask) == info.Value)
                    return info.Interpret;

            throw new NotImplementedException($"Instruction \"{Convert.ToString(Opcode, 2).PadLeft(32, '0')}\" isn't a implemented MIPS instruction.");
        }

        private static void SetOpcode(string Encoding, InstInterp.InterpretOpcode Interpret)
        {
            uint Bit   = (uint)Encoding.Length - 1;
            uint Value = 0;
            uint XMask = 0;

            for (int Index = 0; Index < Encoding.Length; ++Index, --Bit)
            {
                char Chr = Encoding.ToUpper()[Index];

                if (Chr == '1')
                    Value |= (uint)(1 << (int)Bit);
                else if (Chr == 'X')
                    XMask |= (uint)(1 << (int)Bit);
                else if (Chr != '0')
                    throw new ArgumentException(nameof(Encoding));
            }

            XMask = ~XMask;

            AllInsts.Add(new InstInfo(XMask, Value, Interpret));
        }
    }
}
