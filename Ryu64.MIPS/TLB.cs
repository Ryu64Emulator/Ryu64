using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class TLB
    {
        public struct TLBEntry
        {
            public uint PFN;
            public byte PageCoherency;
            public byte Dirty;
            public byte Valid;
            public byte Global;
            public uint VPN2;
            public byte ASID;
            public ushort PageMask;
            public bool HasBeenSet;
        }

        private readonly static TLBEntry[] TLBEntries = new TLBEntry[32];

        public static uint TranslateAddress(uint Address)
        {
            foreach (TLBEntry Entry in TLBEntries)
            {
                if (!Entry.HasBeenSet) continue;

                uint VPN      = (uint)(((Entry.VPN2 << 13) | Entry.ASID) & ~((Entry.PageMask << 13) | 0x1FFF));
                uint Mask     = (uint)((Entry.PageMask << 12) | 0x0FFF);
                uint PageSize = Mask + 1;

                if ((Address & VPN) != VPN) continue;

                if (Entry.Valid == 0) continue;

                return 0x80000000 | (Entry.PFN * PageSize) | (Address & Mask);
            }

            return Address;
        }

        public static void WriteTLBEntryIndexed()
        {
            WriteTLBEntry((uint)Registers.COP0.Reg[Registers.COP0.INDEX_REG]);
        }

        public static void WriteTLBEntryRandom()
        {
            WriteTLBEntry((uint)Registers.COP0.Reg[Registers.COP0.RANDOM_REG]);
        }

        public static void ReadTLBEntry()
        {
            TLBEntry Entry = TLBEntries[(uint)Registers.COP0.Reg[Registers.COP0.INDEX_REG] & 0x3F];
            uint EntryLo = (Entry.PFN << 6)
                           | (byte)(Entry.Global & 0x1)
                           | (byte)((Entry.Valid & 0x1) << 1)
                           | (byte)((Entry.Dirty & 0x1) << 2)
                           | (byte)((Entry.PageCoherency & 0b111) << 3);
            Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] = EntryLo;
            Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] = EntryLo;

            Registers.COP0.Reg[Registers.COP0.PAGEMASK_REG] = (uint)(Entry.PageMask << 13);
            Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] = (Entry.VPN2 << 13) | Entry.ASID;
        }

        private static void WriteTLBEntry(uint Index)
        {
            TLBEntries[Index & 0x3F] = new TLBEntry()
            {
                PFN           = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x3FFFFFC0) >> 6),
                Valid         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b000010)   >> 1),
                Dirty         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b000100)   >> 2),
                PageCoherency = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b111000)   >> 3),
                VPN2          = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] & 0xFFFFE000)  >> 13),
                PageMask      = (ushort)((Registers.COP0.Reg[Registers.COP0.PAGEMASK_REG] & 0x1FFF)   >> 13),
                Global        = (byte)(((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x1)
                                     & ((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x1)),
                ASID = (byte)(Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] & 0xFF),
                HasBeenSet = true
            };
        }

        public static void ProbeTLB()
        {
            for (uint i = 0; i < TLBEntries.Length; ++i)
            {
                TLBEntry Entry = TLBEntries[i];

                if (!Entry.HasBeenSet) continue;

                uint EntryHi = (uint)Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG];
                uint VPN2 = EntryHi & 0xFFFFE000;
                uint ASID = EntryHi & 0xFF;

                if (Entry.VPN2 == VPN2 && Entry.ASID == ASID)
                {
                    Registers.COP0.Reg[Registers.COP0.INDEX_REG] = i & 0x3F;
                    break;
                }
            }
        }
    }
}
