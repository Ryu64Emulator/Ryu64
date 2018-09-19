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
            public bool CanRead;
            public bool CanWrite;
            public string Name;

            public MemEntry(ulong StartAddress, ulong EndAddress, byte[] OutputArray, bool CanRead, bool CanWrite, string Name)
            {
                this.StartAddress = StartAddress;
                this.EndAddress   = EndAddress;
                this.OutputArray  = OutputArray;
                this.CanRead  = CanRead;
                this.CanWrite = CanWrite;
                this.Name     = Name;
            }
        }

        struct MirrorEntry
        {
            public ulong StartAddress, EndAddress, RemapAddress;

            public MirrorEntry(ulong StartAddress, ulong EndAddress, ulong RemapAddress)
            {
                this.StartAddress = StartAddress;
                this.EndAddress   = EndAddress;
                this.RemapAddress = RemapAddress;
            }
        }

        private static readonly byte[] RDRAM       = new byte[8388608];
        private static readonly byte[] RDRAMReg    = new byte[1048575];
        private static readonly byte[] SPReg       = new byte[65535];
        private static readonly byte[] DPCMDReg    = new byte[1048575];
        private static readonly byte[] DPSPANREG   = new byte[1048575];
        private static readonly byte[] MIREG       = new byte[1048575];
        private static readonly byte[] VIREG       = new byte[1048575];
        private static readonly byte[] AIREG       = new byte[1048575];
        private static readonly byte[] PIREG       = new byte[1048575];
        private static readonly byte[] RIREG       = new byte[1048575];
        private static readonly byte[] SIREG       = new byte[1048575];
        

        private static List<MemEntry>    MemoryMap = new List<MemEntry>();
        private static List<MirrorEntry> MirrorMap = new List<MirrorEntry>();

        public static void Init(byte[] Rom)
        {
            // Main Memory Map Entries
            MemoryMap.Add(new MemEntry(0x00000000, 0x007FFFFF,  RDRAM,     true, true, "RDRAM"));
            MemoryMap.Add(new MemEntry(0x03F00000, 0x03FFFFFF,  RDRAMReg,  true, true, "RDRAM Registers"));
            MemoryMap.Add(new MemEntry(0x04000000, 0x0400FFFF,  SPReg,     true, true, "SP Registers"));
            MemoryMap.Add(new MemEntry(0x04100000, 0x041FFFFF,  DPCMDReg,  true, true, "DP Command Registers"));
            MemoryMap.Add(new MemEntry(0x04200000, 0x042FFFFF,  DPSPANREG, true, true, "DP Span Registers"));
            MemoryMap.Add(new MemEntry(0x04300000, 0x043FFFFF,  MIREG,     true, true, "MI Registers"));
            MemoryMap.Add(new MemEntry(0x04400000, 0x044FFFFF,  VIREG,     true, true, "VI Registers"));
            MemoryMap.Add(new MemEntry(0x04500000, 0x045FFFFF,  AIREG,     true, true, "AI Registers"));
            MemoryMap.Add(new MemEntry(0x04600000, 0x046FFFFF,  PIREG,     true, true, "PI Registers"));
            MemoryMap.Add(new MemEntry(0x04700000, 0x047FFFFF,  RIREG,     true, true, "RI Registers"));
            MemoryMap.Add(new MemEntry(0x04800000, 0x048FFFFF,  SIREG,     true, true, "SI Registers"));
            MemoryMap.Add(new MemEntry(0x10000000, 0x1F39FFFF,  Rom,       true, true, "Cartridge Domain 1 (Address 2)"));

            // Mirror Entry
            MirrorMap.Add(new MirrorEntry(0x80000000, 0x9FFFFFFF, 0x00000000)); // kseg0
            MirrorMap.Add(new MirrorEntry(0xA0000000, 0xBFFFFFFF, 0x00000000)); // kseg1
            // TODO: Add the TLB mapped regions (ksseg and kseg3) plus emulate the TLB.

            // Set up Memory Registers
            WriteUInt32(0x0470000C, 14); // RI_SELECT_REG
        }

        public static byte ReadUInt8(ulong Position)
        {
            ulong TranslatedPosition = Position;
            foreach (MirrorEntry CurrentEntry in MirrorMap)
            {
                if (Position >= CurrentEntry.StartAddress && Position <= CurrentEntry.EndAddress)
                {
                    TranslatedPosition = (Position - CurrentEntry.StartAddress) + CurrentEntry.RemapAddress;
                    break;
                }
            }

            MemEntry MemoryEntry = new MemEntry();
            bool FoundMemEntry = false;

            foreach (MemEntry CurrentEntry in MemoryMap)
            {
                if (TranslatedPosition >= CurrentEntry.StartAddress && TranslatedPosition <= CurrentEntry.EndAddress)
                {
                    MemoryEntry = CurrentEntry;
                    FoundMemEntry = true;
                    break;
                }
            }

            if (!FoundMemEntry)
            {
                throw new Common.Exceptions.InvalidOrUnimplementedMemoryMapException($"\"0x{TranslatedPosition:x8}\" is not within range of any mapped memory.");
            }
            else if (!MemoryEntry.CanRead)
            {
                throw new Common.Exceptions.MemoryProtectionViolation($"\"{MemoryEntry.Name}\" (0x{MemoryEntry.StartAddress:x8} to 0x{MemoryEntry.EndAddress:x8}) does not have read privileges.  Address at: 0x{TranslatedPosition:x8}");
            }

            return MemoryEntry.OutputArray[TranslatedPosition - MemoryEntry.StartAddress];
        }

        public static void WriteUInt8(ulong Position, byte Value)
        {
            ulong TranslatedPosition = Position;
            foreach (MirrorEntry CurrentEntry in MirrorMap)
            {
                if (Position >= CurrentEntry.StartAddress && Position <= CurrentEntry.EndAddress)
                {
                    TranslatedPosition = (Position - CurrentEntry.StartAddress) + CurrentEntry.RemapAddress;
                    break;
                }
            }

            MemEntry MemoryEntry = new MemEntry();
            bool FoundMemEntry = false;

            foreach (MemEntry CurrentEntry in MemoryMap)
            {
                if (TranslatedPosition >= CurrentEntry.StartAddress && TranslatedPosition <= CurrentEntry.EndAddress)
                {
                    MemoryEntry = CurrentEntry;
                    FoundMemEntry = true;
                    break;
                }
            }

            if (!FoundMemEntry)
            {
                throw new Common.Exceptions.InvalidOrUnimplementedMemoryMapException($"\"0x{TranslatedPosition:x8}\" is not within range of any mapped memory.");
            }
            else if (!MemoryEntry.CanWrite)
            {
                throw new Common.Exceptions.MemoryProtectionViolation($"\"{MemoryEntry.Name}\" (0x{MemoryEntry.StartAddress:x8} to 0x{MemoryEntry.EndAddress:x8}) does not have write privileges.  Address at: 0x{TranslatedPosition:x8}");
            }

            MemoryEntry.OutputArray[TranslatedPosition - MemoryEntry.StartAddress] = Value;
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
            return (ushort)(ReadUInt8(Position) << 8 | ReadUInt8(Position + 1));
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
