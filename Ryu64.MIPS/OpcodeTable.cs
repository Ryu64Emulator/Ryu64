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

        private static List<InstInfo> AllInsts;

        public static void Init()
        {
            AllInsts = new List<InstInfo>();

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
