using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class OpcodeTable
    {
        public struct InstInfo
        {
            public uint Mask;
            public uint Value;
            public string FormattedASM;

            public InstInterp.InterpretOpcode Interpret;

            public InstInfo(uint Mask, uint Value, InstInterp.InterpretOpcode Interpret, string FormattedASM)
            {
                this.Mask         = Mask;
                this.Value        = Value;
                this.Interpret    = Interpret;
                this.FormattedASM = FormattedASM;
            }
        }

        public struct OpcodeDesc
        {
            public uint Opcode;

            public byte   op1;
            public byte   op2;
            public byte   op3;
            public byte   op4;
            public ushort Imm;
            public uint   Target;
            public uint   ExceptionCode20bit;
            public uint   ExceptionCode10bit;

            public OpcodeDesc(uint Opcode)
            {
                this.Opcode = Opcode;

                op1 = (byte)        ((Opcode & 0b00000011111000000000000000000000) >> 21);
                op2 = (byte)        ((Opcode & 0b00000000000111110000000000000000) >> 16);
                op3 = (byte)        ((Opcode & 0b00000000000000001111100000000000) >> 11);
                op4 = (byte)        ((Opcode & 0b00000000000000000000011111000000) >> 6);
                Imm = (ushort)      ((Opcode & 0b00000000000000001111111111111111));
                Target             =((Opcode & 0b00000011111111111111111111111111) << 2);
                ExceptionCode20bit = (Opcode & 0b00000011111111111111111111000000) >> 6;
                ExceptionCode10bit = (Opcode & 0b00000000000000001111111111000000) >> 6;
            }
        }

        private static List<InstInfo> AllInsts;

        public static void Init()
        {
            AllInsts = new List<InstInfo>();

            /*
            Note:
            The Formatting for the Assembly string goes as follows:
            {0} is the op1 part of a OpcodeDesc
            {1} is the op2 part of a OpcodeDesc
            {2} is the op3 part of a OpcodeDesc
            {3} is the op4 part of a OpcodeDesc
            {4} is the Imm part of a OpcodeDesc
            {5} is the Target part of a OpcodeDesc
            {6} is the Exception Code (20-bit) part of a OpcodeDesc
            {7} is the Exception Code (10-bit) part of a OpcodeDesc
            */

            // Other Instructions
            SetOpcode("00000000000000000000000000000000", InstInterp.NOP, "NOP");

            // Load / Store Instructions
            SetOpcode("100000XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LB,  "LB R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LBU, "LBU R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("110111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LD,  "LD R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("011010XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LDL, "LDL R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("011011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LDR, "LDR R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100001XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LH,  "LH R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LHU, "LHU R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LW,  "LW R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100010XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LWL, "LWL R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100110XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LWR, "LWR R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("100111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.LWU, "LWU R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101000XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SB,  "SB R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("111111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SD,  "SD R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SDL, "SDL R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SDR, "SDR R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101001XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SH,  "SH R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SW,  "SW R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101010XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SWL, "SWL R[{1}], 0x{4:x4}(R[{0}])");
            SetOpcode("101110XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SWR, "SWR R[{1}], 0x{4:x4}(R[{0}])");

            // Arithmetic Instructions
            SetOpcode("000000XXXXXXXXXXXXXXX00000100000", InstInterp.ADD,   "ADD R[{2}], R[{0}], R[{1}]");
            SetOpcode("001000XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.ADDI,  "ADDI R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("001001XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.ADDIU, "ADDIU R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("001100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.ANDI,  "ANDI R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("000000XXXXXXXXXXXXXXX00000100110", InstInterp.XOR,   "XOR R[{2}], R[{0}], R[{1}]");
            SetOpcode("001110XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.XORI,  "XORI R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("00111100000XXXXXXXXXXXXXXXXXXXXX", InstInterp.LUI,   "LUI R[{1}], 0x{4:x4}");
            SetOpcode("000000XXXXXXXXXXXXXXX00000100101", InstInterp.OR,    "OR R[{2}], R[{0}], R[{1}]");
            SetOpcode("001101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.ORI,   "ORI R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("00000000000XXXXXXXXXXXXXXX000000", InstInterp.SLL,   "SLL R[{2}], R[{1}], 0x{3:x2}");
            SetOpcode("00000000000XXXXXXXXXXXXXXX000010", InstInterp.SRL,   "SRL R[{2}], R[{1}], 0x{3:x2}");
            SetOpcode("001010XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SLTI,  "SLTI R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("001011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.SLTIU, "SLTIU R[{1}], R[{0}], 0x{4:x4}");
            SetOpcode("000000XXXXXXXXXXXXXXX00000101011", InstInterp.SLTU,  "SLTU R[{2}], R[{0}], R[{1}]");

            // Branch / Jump Instructions
            SetOpcode("000100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.BEQ,   "BEQ R[{0}], R[{1}], 0x{4:x4}");
            SetOpcode("010100XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.BEQL,  "BEQL R[{0}], R[{1}], 0x{4:x4}");
            SetOpcode("010110XXXXX00000XXXXXXXXXXXXXXXX", InstInterp.BLEZL, "BLEZL R[{0}], 0x{4:x4}");
            SetOpcode("000101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.BNE,   "BNE R[{0}], R[{1}], 0x{4:x4}");
            SetOpcode("010101XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.BNEL,  "BNEL R[{0}], R[{1}], 0x{4:x4}");
            SetOpcode("000011XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.JAL,   "JAL 0x{5:x8}");
            SetOpcode("000000XXXXX000000000000000001000", InstInterp.JR,    "JR R[{0}]");

            // COP0 Instructions
            SetOpcode("01000000000XXXXXXXXXXXXXXXXXXXXX", InstInterp.MFC0,  "MFC0 R[{1}], CP0R[{2}]");
            SetOpcode("01000000100XXXXXXXXXXXXXXXXXXXXX", InstInterp.MTC0,  "MTC0 R[{1}], CP0R[{2}]");
            SetOpcode("101111XXXXXXXXXXXXXXXXXXXXXXXXXX", InstInterp.CACHE, "CACHE 0x{1:x2}, 0x{4:x4}(R[{0}])");
        }

        public static InstInfo GetOpcodeInfo(uint Opcode)
        {
            foreach (InstInfo info in AllInsts)
                if ((Opcode & info.Mask) == info.Value)
                    return info;

            throw new NotImplementedException($"Instruction \"{Convert.ToString(Opcode, 2).PadLeft(32, '0')}\" isn't a implemented MIPS instruction.");
        }

        private static void SetOpcode(string Encoding, InstInterp.InterpretOpcode Interpret, string FormattedASM = "")
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

            AllInsts.Add(new InstInfo(XMask, Value, Interpret, FormattedASM));
        }
    }
}
