using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public static class Memory
    {

        struct MemEntry
        {
            public ulong StartAddress;
            public ulong EndAddress;
            public byte[] OutputArray;
            public bool   CanRead;
            public bool   CanWrite;
            public string Name;

            public MemEntry(ulong StartAddress, ulong EndAddress, byte[] OutputArray, bool CanRead, bool CanWrite, string Name)
            {
                this.StartAddress = StartAddress;
                this.EndAddress   = EndAddress;
                this.OutputArray  = OutputArray;
                this.CanRead      = CanRead;
                this.CanWrite     = CanWrite;
                this.Name         = Name;
            }
        }

        public const ulong RI_SELECT_REG = 0x0470000C;
        public const ulong SP_STATUS_REG = 0x04040010;

        private static readonly byte[] RDRAM     = new byte[8388608];
        private static readonly byte[] RDRAMReg  = new byte[1048575];
        private static readonly byte[] SPReg     = new byte[1048575];
        private static readonly byte[] DPCMDReg  = new byte[1048575];
        private static readonly byte[] DPSPANREG = new byte[1048575];
        private static readonly byte[] MIREG     = new byte[1048575];
        private static readonly byte[] VIREG     = new byte[1048575];
        private static readonly byte[] AIREG     = new byte[1048575];
        private static readonly byte[] PIREG     = new byte[1048575];
        private static readonly byte[] RIREG     = new byte[1048575];
        private static readonly byte[] SIREG     = new byte[1048575];
        private static readonly byte[] PIFROM    = new byte[1984];
        private static readonly byte[] PIFRAM    = new byte[64];

        private static List<MemEntry>    MemoryMap = new List<MemEntry>();

        public static void Init(byte[] Rom)
        {
            // Main Memory Map Entries
            MemoryMap.Add(new MemEntry(0x00000000, 0x007FFFFF,  RDRAM,     true, true, "RDRAM"));
            MemoryMap.Add(new MemEntry(0x03F00000, 0x03FFFFFF,  RDRAMReg,  true, true, "RDRAM Registers"));
            MemoryMap.Add(new MemEntry(0x04000000, 0x040FFFFF,  SPReg,     true, true, "SP Registers"));
            MemoryMap.Add(new MemEntry(0x04100000, 0x041FFFFF,  DPCMDReg,  true, true, "DP Command Registers"));
            MemoryMap.Add(new MemEntry(0x04200000, 0x042FFFFF,  DPSPANREG, true, true, "DP Span Registers"));
            MemoryMap.Add(new MemEntry(0x04300000, 0x043FFFFF,  MIREG,     true, true, "MI Registers"));
            MemoryMap.Add(new MemEntry(0x04400000, 0x044FFFFF,  VIREG,     true, true, "VI Registers"));
            MemoryMap.Add(new MemEntry(0x04500000, 0x045FFFFF,  AIREG,     true, true, "AI Registers"));
            MemoryMap.Add(new MemEntry(0x04600000, 0x046FFFFF,  PIREG,     true, true, "PI Registers"));
            MemoryMap.Add(new MemEntry(0x04700000, 0x047FFFFF,  RIREG,     true, true, "RI Registers"));
            MemoryMap.Add(new MemEntry(0x04800000, 0x048FFFFF,  SIREG,     true, true, "SI Registers"));
            MemoryMap.Add(new MemEntry(0x10000000, 0x1F39FFFF,  Rom,       true, true, "Cartridge Domain 1 (Address 2)"));
            MemoryMap.Add(new MemEntry(0x1FC00000, 0x1FC007BF,  PIFROM,    true, true, "PIF Rom"));
            MemoryMap.Add(new MemEntry(0x1FC007C0, 0x1FC007FF,  PIFRAM,    true, true, "PIF Ram"));

            // Set up Memory Registers
            if (!Common.Settings.LOAD_PIF)
                WriteUInt32(RI_SELECT_REG, 14);
            WriteUInt32(SP_STATUS_REG, 1);
        }

        public static byte ReadUInt8(ulong Position, bool IsPositionAlreadyTranslated = false)
        {
            ulong TranslatedPosition = Position & 0x1FFFFFFF;

            MemEntry MemoryEntry = new MemEntry();
            bool FoundMemEntry   = false;

            foreach (MemEntry CurrentEntry in MemoryMap)
            {
                if (TranslatedPosition > CurrentEntry.EndAddress || TranslatedPosition < CurrentEntry.StartAddress) continue;

                if (TranslatedPosition <= CurrentEntry.EndAddress)
                {
                    MemoryEntry = CurrentEntry;
                    FoundMemEntry = true;
                    break;
                }
            }

            if (!FoundMemEntry)
            {
                throw new Common.Exceptions.InvalidOrUnimplementedMemoryMapException($"\"0x{TranslatedPosition:x16}\" is not within range of any mapped memory.");
            }
            else if (!MemoryEntry.CanRead)
            {
                throw new Common.Exceptions.MemoryProtectionViolation($"\"{MemoryEntry.Name}\" (0x{MemoryEntry.StartAddress:x16} to 0x{MemoryEntry.EndAddress:x16}) does not have read privileges.  Address at: 0x{TranslatedPosition:x16}");
            }

            ulong FullAddress = TranslatedPosition - MemoryEntry.StartAddress;
            if (Common.Settings.LOG_MEM_ACCESS) Common.Logger.PrintInfoLine($"Tried to Read at Address \"0x{FullAddress:x16}\", Section \"{MemoryEntry.Name}\"");

            return MemoryEntry.OutputArray[FullAddress];
        }

        public static void WriteUInt8(ulong Position, byte Value)
        {
            ulong TranslatedPosition = Position & 0x1FFFFFFF;

            MemEntry MemoryEntry = new MemEntry();
            bool FoundMemEntry   = false;

            foreach (MemEntry CurrentEntry in MemoryMap)
            {
                if (TranslatedPosition > CurrentEntry.EndAddress || TranslatedPosition < CurrentEntry.StartAddress) continue;

                if (TranslatedPosition <= CurrentEntry.EndAddress)
                {
                    MemoryEntry = CurrentEntry;
                    FoundMemEntry = true;
                    break;
                }
            }

            if (!FoundMemEntry)
            {
                throw new Common.Exceptions.InvalidOrUnimplementedMemoryMapException($"\"0x{TranslatedPosition:x16}\" is not within range of any mapped memory.");
            }
            else if (!MemoryEntry.CanWrite)
            {
                throw new Common.Exceptions.MemoryProtectionViolation($"\"{MemoryEntry.Name}\" (0x{MemoryEntry.StartAddress:x16} to 0x{MemoryEntry.EndAddress:x16}) does not have write privileges.  Address at: 0x{TranslatedPosition:x16}");
            }

            ulong FullAddress = TranslatedPosition - MemoryEntry.StartAddress;
            if (Common.Settings.LOG_MEM_ACCESS) Common.Logger.PrintInfoLine($"Tried to Write at Address \"0x{FullAddress:x16}\", Value \"0x{Value:x16}\", Section \"{MemoryEntry.Name}\"");

            MemoryEntry.OutputArray[FullAddress] = Value;
        }

        public static sbyte ReadInt8(ulong Position)
        {
            return (sbyte)ReadUInt8(Position);
        }

        public static void WriteInt8(ulong Position, sbyte Value)
        {
            WriteUInt8(Position, (byte)Value);
        }

        public static ushort ReadUInt16(ulong Position)
        {
            return (ushort)(ReadUInt8(Position, true) << 8 | ReadUInt8(Position + 1, true));
        }

        public static void WriteUInt16(ulong Position, ushort Value)
        {
            WriteUInt8(Position,     (byte)((Value & 0xFF00) >> 8));
            WriteUInt8(Position + 1, (byte) (Value & 0x00FF));
        }

        public static short ReadInt16(ulong Position)
        {
            return (short)ReadUInt16(Position);
        }

        public static void WriteInt16(ulong Position, short Value)
        {
            WriteUInt16(Position, (ushort)Value);
        }

        public static uint ReadUInt32(ulong Position)
        {
            return (uint)(ReadUInt16(Position) << 16 | ReadUInt16(Position + 2));
        }

        public static void WriteUInt32(ulong Position, uint Value)
        {
            WriteUInt16(Position,     (ushort)((Value & 0xFFFF0000) >> 16));
            WriteUInt16(Position + 2, (ushort) (Value & 0x0000FFFF));
        }

        public static int ReadInt32(ulong Position)
        {
            return (int)ReadUInt32(Position);
        }

        public static void WriteInt32(ulong Position, int Value)
        {
            WriteUInt32(Position, (uint)Value);
        }

        public static ulong ReadUInt64(ulong Position)
        {
            return ReadUInt32(Position) << 32 | ReadUInt32(Position + 4);
        }

        public static void WriteUInt64(ulong Position, ulong Value)
        {
            WriteUInt32(Position,     (uint)((Value & 0xFFFFFFFF00000000) >> 32));
            WriteUInt32(Position + 4, (uint) (Value & 0x00000000FFFFFFFF));
        }

        public static long ReadInt64(ulong Position)
        {
            return (long)ReadUInt64(Position);
        }

        public static void WriteInt64(ulong Position, long Value)
        {
            WriteUInt64(Position, (ulong)Value);
        }
    }
}
