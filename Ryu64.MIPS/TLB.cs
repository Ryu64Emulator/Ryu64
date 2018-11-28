using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class TLB
    {
        public struct TLBEntry
        {
            public uint PFN0;
            public byte PageCoherency0;
            public byte Dirty0;
            public byte Valid0;
            public byte Global0;
            public uint PFN1;
            public byte PageCoherency1;
            public byte Dirty1;
            public byte Valid1;
            public byte Global1;
            public uint VPN2;
            public byte ASID;
            public ushort PageMask;
        }

        private readonly static TLBEntry[] TLBEntries = new TLBEntry[32];

        public static uint TranslateAddress(uint Address)
        {
            foreach (TLBEntry Entry in TLBEntries)
            {
                uint VPN      = (uint)(((Entry.VPN2 << 13) | Entry.ASID) & ~((Entry.PageMask << 13) | 0x1FFF));
                uint Mask     = (uint)((Entry.PageMask << 12) | 0x0FFF);
                uint PageSize = Mask + 1;

                if ((Address & (VPN | 0xE0000000)) != VPN) continue;

                uint Valid = ((Address & PageSize) != 1) ? Entry.Valid0 : Entry.Valid1;

                if (Valid == 0) continue;

                uint PFN = ((Address & PageSize) != 1) ? Entry.PFN0 : Entry.PFN1;

                return (PFN * PageSize) | (Address & Mask);
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
            Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] = (Entry.PFN0 << 6)
                                                               | (byte)(Entry.Global0 & 0x1)
                                                               | (byte)((Entry.Valid0 & 0x1) << 1)
                                                               | (byte)((Entry.Dirty0 & 0x1) << 2)
                                                               | (byte)((Entry.PageCoherency0 & 0b111) << 3);
            Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] = (Entry.PFN1 << 6)
                                                               | (byte)(Entry.Global1 & 0x1)
                                                               | (byte)((Entry.Valid1 & 0x1) << 1)
                                                               | (byte)((Entry.Dirty1 & 0x1) << 2)
                                                               | (byte)((Entry.PageCoherency1 & 0b111) << 3);

            Registers.COP0.Reg[Registers.COP0.PAGEMASK_REG] = (uint)(Entry.PageMask << 13);
            Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] = (Entry.VPN2 << 13) | Entry.ASID;
        }

        private static void WriteTLBEntry(uint Index)
        {
            TLBEntries[Index & 0x3F] = new TLBEntry()
            {
                PFN0           = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x3FFFFFC0) >> 6),
                Valid0         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0b000010)   >> 1),
                Dirty0         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0b000100)   >> 2),
                PageCoherency0 = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0b111000)   >> 3),
                PFN1           = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x3FFFFFC0) >> 6),
                Valid1         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b000010) >> 1),
                Dirty1         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b000100) >> 2),
                PageCoherency1 = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b111000) >> 3),
                VPN2          = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] & 0xFFFFE000)  >> 13),
                PageMask      = (ushort)((Registers.COP0.Reg[Registers.COP0.PAGEMASK_REG] & 0x1FFF)   >> 13),
                Global0       = (byte)(((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x1)
                                     & ((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x1)),
                Global1       = (byte)(((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x1)
                                     & ((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x1)),
                ASID = (byte)(Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] & 0xFF)
            };
        }

        public static void ProbeTLB()
        {
            bool FoundEntry = false;
            for (uint i = 0; i < TLBEntries.Length; ++i)
            {
                TLBEntry Entry = TLBEntries[i];

                if ((Entry.Valid0 | Entry.Valid1) == 0) continue;

                uint EntryHi = (uint)Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG];
                uint VPN2 = (EntryHi & 0xFFFFE000) >> 13;
                uint ASID = EntryHi & 0xFF;

                if (Entry.VPN2 == VPN2 && Entry.ASID == ASID)
                {
                    FoundEntry = true;
                    Registers.COP0.Reg[Registers.COP0.INDEX_REG] = i & 0x3F;
                    break;
                }
            }

            if (!FoundEntry) throw new Common.Exceptions.TLBMissException("TLB Miss happened while probing the TLB.");
        }
    }
}
