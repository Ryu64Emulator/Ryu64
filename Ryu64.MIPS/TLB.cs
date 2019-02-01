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
            public uint EntryHi;
            public uint PageMask;
        }

        public readonly static TLBEntry[] TLBEntries = new TLBEntry[32];

        private readonly static Dictionary<uint, uint> AddressTranslationCache = new Dictionary<uint, uint>();

        private readonly static List<uint> EntriesWrittenToCache = new List<uint>();

        public static uint TranslateAddress(uint Address)
        {
            if ((Address & 0xC0000000) == 0x80000000)
                return Address;

            if (AddressTranslationCache.ContainsKey(Address))
                return AddressTranslationCache[Address];

            for (uint i = 0; i < EntriesWrittenToCache.Count; ++i)
            {
                TLBEntry Entry = TLBEntries[EntriesWrittenToCache[(int)i]];

                uint VPN = Entry.EntryHi & ~(Entry.PageMask | 0x1FFF);

                if ((Address & (VPN | 0xE0000000)) != VPN) continue;
                
                uint Mask     = (Entry.PageMask >> 1) | 0x0FFF;
                uint PageSize = Mask + 1;

                bool Odd = (Address & PageSize) == 1;

                uint Valid = (!Odd) ? Entry.Valid0 : Entry.Valid1;

                if (Valid == 0) continue;

                uint PFN = (!Odd) ? Entry.PFN0 : Entry.PFN1;

                uint Result = (PFN * PageSize) | (Address & Mask);

                if (!AddressTranslationCache.ContainsKey(Address))
                    AddressTranslationCache.Add(Address, Result);

                return Result;
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
            TLBEntry Entry = TLBEntries[(uint)Registers.COP0.Reg[Registers.COP0.INDEX_REG] & 0x1F];
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
            Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG] = Entry.EntryHi;
        }

        private static void WriteTLBEntry(uint Index)
        {
            TLBEntries[Index & 0x1F] = new TLBEntry()
            {
                PFN0           = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x3FFFFFC0) >> 6),
                Valid0         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0b000010)   >> 1),
                Dirty0         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0b000100)   >> 2),
                PageCoherency0 = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0b111000)   >> 3),
                PFN1           = (uint)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x3FFFFFC0) >> 6),
                Valid1         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b000010) >> 1),
                Dirty1         = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b000100) >> 2),
                PageCoherency1 = (byte)((Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0b111000) >> 3),
                EntryHi       = (uint)Registers.COP0.Reg[Registers.COP0.ENTRYHI_REG],
                PageMask      = (uint)Registers.COP0.Reg[Registers.COP0.PAGEMASK_REG],
                Global0       = (byte)(((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x1)
                                     & ((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x1)),
                Global1       = (byte)(((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO0_REG] & 0x1)
                                     & ((byte)Registers.COP0.Reg[Registers.COP0.ENTRYLO1_REG] & 0x1)),
            };

            if (!EntriesWrittenToCache.Contains(Index & 0x1F))
                EntriesWrittenToCache.Add(Index & 0x1F);

            AddressTranslationCache.Clear();
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
                uint EntryVPN2 = (Entry.EntryHi & 0xFFFFE000) >> 13;
                uint EntryASID = Entry.EntryHi & 0xFF;

                if (EntryVPN2 == VPN2 && EntryASID == ASID)
                {
                    FoundEntry = true;
                    Registers.COP0.Reg[Registers.COP0.INDEX_REG] = i & 0x3F;
                    break;
                }
            }

            if (!FoundEntry) Registers.COP0.Reg[Registers.COP0.INDEX_REG] |= 0x80000000;
        }
    }
}
