using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ryu64.MIPS
{
    public class Memory
    {
        private delegate void MemoryEvent();

        public readonly byte[] SP_DMEM_RW         = new byte[0x1000];
        public readonly byte[] SP_IMEM_RW         = new byte[0x1000];
        public readonly byte[] SP_STATUS_REG_R    = new byte[4];
        public readonly byte[] SP_STATUS_REG_W    = new byte[4];
        public readonly byte[] SP_DMA_BUSY_REG_R  = new byte[4];
        public readonly byte[] SP_DMA_BUSY_REG_W  = new byte[4];
        public readonly byte[] SP_SEMAPHORE_REG_R = new byte[4];
        public readonly byte[] SP_SEMAPHORE_REG_W = new byte[4];
        public readonly byte[] SP_PC_REG_RW       = new byte[4];

        public readonly byte[] DPC_STATUS_REG_R = new byte[4];
        public readonly byte[] DPC_STATUS_REG_W = new byte[4];

        public readonly byte[] MI_INIT_MODE_REG_R = new byte[4];
        public readonly byte[] MI_INIT_MODE_REG_W = new byte[4];
        public readonly byte[] MI_VERSION_REG_RW  = new byte[4];
        public readonly byte[] MI_INTR_REG_R      = new byte[4];
        public readonly byte[] MI_INTR_MASK_REG_R = new byte[4];
        public readonly byte[] MI_INTR_MASK_REG_W = new byte[4];

        public readonly byte[] VI_STATUS_REG_RW  = new byte[4];
        public readonly byte[] VI_ORIGIN_REG_RW  = new byte[4];
        public readonly byte[] VI_WIDTH_REG_RW   = new byte[4];
        public readonly byte[] VI_INTR_REG_RW    = new byte[4];
        public readonly byte[] VI_CURRENT_REG_RW = new byte[4];
        public readonly byte[] VI_BURST_REG_RW   = new byte[4];
        public readonly byte[] VI_V_SYNC_REG_RW  = new byte[4];
        public readonly byte[] VI_H_SYNC_REG_RW  = new byte[4];
        public readonly byte[] VI_LEAP_REG_RW    = new byte[4];
        public readonly byte[] VI_H_START_REG_RW = new byte[4];
        public readonly byte[] VI_V_START_REG_RW = new byte[4];
        public readonly byte[] VI_V_BURST_REG_RW = new byte[4];
        public readonly byte[] VI_X_SCALE_REG_RW = new byte[4];
        public readonly byte[] VI_Y_SCALE_REG_RW = new byte[4];

        public readonly byte[] AI_DRAM_ADDR_REG_W = new byte[4];
        public readonly byte[] AI_LEN_REG_RW      = new byte[4];
        public readonly byte[] AI_CONTROL_REG_W   = new byte[4];
        public readonly byte[] AI_STATUS_REG_R    = new byte[4];
        public readonly byte[] AI_STATUS_REG_W    = new byte[4];
        public readonly byte[] AI_DACRATE_REG_W   = new byte[4];
        public readonly byte[] AI_BITRATE_REG_W   = new byte[4];

        public readonly byte[] PI_DRAM_ADDR_REG_RW    = new byte[4];
        public readonly byte[] PI_CART_ADDR_REG_RW    = new byte[4];
        public readonly byte[] PI_WR_LEN_REG_RW       = new byte[4];
        public readonly byte[] PI_STATUS_REG_R        = new byte[4];
        public readonly byte[] PI_STATUS_REG_W        = new byte[4];
        public readonly byte[] PI_BSD_DOM1_LAT_REG_RW = new byte[4];
        public readonly byte[] PI_BSD_DOM1_PWD_REG_RW = new byte[4];
        public readonly byte[] PI_BSD_DOM1_PGS_REG_RW = new byte[4];
        public readonly byte[] PI_BSD_DOM1_RLS_REG_RW = new byte[4];

        public readonly byte[] SI_STATUS_REG_R  = new byte[4];
        public readonly byte[] SI_STATUS_REG_W  = new byte[4];

        public readonly byte[] RI_SELECT_REG_RW = new byte[4];

        public readonly byte[] RDRAM     = new byte[8388608];
        public readonly byte[] RDRAMReg  = new byte[1048575];
        public readonly byte[] PIFROM    = new byte[1984];
        public readonly byte[] PIFRAM    = new byte[64];

        private List<MemEntry> MemoryMapList = new List<MemEntry>();
        private MemEntry[]     MemoryMap;

        public Memory(byte[] Rom)
        {
            // RDRAM
            MemoryMapList.Add(new MemEntry(0x00000000, 0x007FFFFF, RDRAM, RDRAM,       "RDRAM"));
            MemoryMapList.Add(new MemEntry(0x03F00000, 0x03FFFFFF, RDRAMReg, RDRAMReg, "RDRAM Registers"));

            // SP Registers
            MemoryMapList.Add(new MemEntry(0x04000000, 0x04000FFF, SP_DMEM_RW,         SP_DMEM_RW,          "SP_DMEM"));
            MemoryMapList.Add(new MemEntry(0x04001000, 0x04001FFF, SP_IMEM_RW,         SP_IMEM_RW,          "SP_IMEM"));
            MemoryMapList.Add(new MemEntry(0x04040010, 0x04040013, SP_STATUS_REG_R,    SP_STATUS_REG_W,     "SP_STATUS_REG"));
            MemoryMapList.Add(new MemEntry(0x04040018, 0x0404001B, SP_DMA_BUSY_REG_R,  SP_DMA_BUSY_REG_W,   "SP_DMA_BUSY_REG"));
            MemoryMapList.Add(new MemEntry(0x0404001C, 0x0404001F, SP_SEMAPHORE_REG_R, SP_SEMAPHORE_REG_W,  "SP_SEMAPHORE_REG"));
            MemoryMapList.Add(new MemEntry(0x04080000, 0x04080003, SP_PC_REG_RW,       SP_PC_REG_RW,        "SP_PC_REG"));

            // DPC Registers
            MemoryMapList.Add(new MemEntry(0x0410000C, 0x0410000F, DPC_STATUS_REG_R, DPC_STATUS_REG_W, "DPC_STATUS_REG"));

            // MI Registers
            MemoryMapList.Add(new MemEntry(0x04300000, 0x04300003, MI_INIT_MODE_REG_R, MI_INIT_MODE_REG_W, "MI_INIT_MODE_REG"));
            MemoryMapList.Add(new MemEntry(0x04300004, 0x04300007, MI_VERSION_REG_RW,  MI_VERSION_REG_RW,  "MI_VERSION_REG"));
            MemoryMapList.Add(new MemEntry(0x04300008, 0x0430000B, MI_INTR_REG_R,      null,               "MI_INTR_REG"));
            MemoryMapList.Add(new MemEntry(0x0430000C, 0x0430000F, MI_INTR_MASK_REG_R, MI_INTR_MASK_REG_W, "MI_INTR_MASK_REG"));

            // VI Registers
            MemoryMapList.Add(new MemEntry(0x04400000, 0x04400003, VI_STATUS_REG_RW,  VI_STATUS_REG_RW,  "VI_STATUS_REG"));
            MemoryMapList.Add(new MemEntry(0x04400004, 0x04400007, VI_ORIGIN_REG_RW,  VI_ORIGIN_REG_RW,  "VI_ORIGIN_REG"));
            MemoryMapList.Add(new MemEntry(0x04400008, 0x0440000B, VI_WIDTH_REG_RW,   VI_WIDTH_REG_RW,   "VI_WIDTH_REG"));
            MemoryMapList.Add(new MemEntry(0x0440000C, 0x0440000F, VI_INTR_REG_RW,    VI_INTR_REG_RW,    "VI_INTR_REG"));
            MemoryMapList.Add(new MemEntry(0x04400010, 0x04400013, VI_CURRENT_REG_RW, VI_CURRENT_REG_RW, "VI_CURRENT_REG"));
            MemoryMapList.Add(new MemEntry(0x04400014, 0x04400017, VI_BURST_REG_RW,   VI_BURST_REG_RW,   "VI_BURST_REG"));
            MemoryMapList.Add(new MemEntry(0x04400018, 0x0440001B, VI_V_SYNC_REG_RW,  VI_V_SYNC_REG_RW,  "VI_V_SYNC_REG"));
            MemoryMapList.Add(new MemEntry(0x0440001C, 0x0440001F, VI_H_SYNC_REG_RW,  VI_H_SYNC_REG_RW,  "VI_H_SYNC_REG"));
            MemoryMapList.Add(new MemEntry(0x04400020, 0x04400023, VI_LEAP_REG_RW,    VI_LEAP_REG_RW,    "VI_LEAP_REG"));
            MemoryMapList.Add(new MemEntry(0x04400024, 0x04400027, VI_H_START_REG_RW, VI_H_START_REG_RW, "VI_H_START_REG"));
            MemoryMapList.Add(new MemEntry(0x04400028, 0x0440002B, VI_V_START_REG_RW, VI_V_START_REG_RW, "VI_V_START_REG"));
            MemoryMapList.Add(new MemEntry(0x0440002C, 0x0440002F, VI_V_BURST_REG_RW, VI_V_BURST_REG_RW, "VI_V_BURST_REG"));
            MemoryMapList.Add(new MemEntry(0x04400030, 0x04400033, VI_X_SCALE_REG_RW, VI_X_SCALE_REG_RW, "VI_X_SCALE_REG"));
            MemoryMapList.Add(new MemEntry(0x04400034, 0x04400037, VI_Y_SCALE_REG_RW, VI_Y_SCALE_REG_RW, "VI_Y_SCALE_REG"));

            // AI Registers
            MemoryMapList.Add(new MemEntry(0x04500000, 0x04500003, null, AI_DRAM_ADDR_REG_W,          "AI_DRAM_ADDR_REG"));
            MemoryMapList.Add(new MemEntry(0x04500004, 0x04500007, AI_LEN_REG_RW,   AI_LEN_REG_RW,    "AI_LEN_REG"));
            MemoryMapList.Add(new MemEntry(0x04500008, 0x0450000B, null,            AI_CONTROL_REG_W, "AI_CONTROL_REG"));
            MemoryMapList.Add(new MemEntry(0x0450000C, 0x0450000F, AI_STATUS_REG_R, AI_STATUS_REG_W,  "AI_STATUS_REG"));
            MemoryMapList.Add(new MemEntry(0x04500010, 0x04500013, null,            AI_DACRATE_REG_W, "AI_DACRATE_REG"));
            MemoryMapList.Add(new MemEntry(0x04500014, 0x04500017, null,            AI_BITRATE_REG_W, "AI_BITRATE_REG"));

            // PI Registers
            MemoryMapList.Add(new MemEntry(0x04600000, 0x04600003, PI_DRAM_ADDR_REG_RW, PI_DRAM_ADDR_REG_RW, "PI_DRAM_ADDR_REG"));
            MemoryMapList.Add(new MemEntry(0x04600004, 0x04600007, PI_CART_ADDR_REG_RW, PI_CART_ADDR_REG_RW, "PI_CART_ADDR_REG"));
            MemoryMapList.Add(new MemEntry(0x0460000C, 0x0460000F, PI_WR_LEN_REG_RW, PI_WR_LEN_REG_RW,       "PI_WR_LEN_REG", 
                null, PI_WR_LEN_WRITE_EVENT));
            MemoryMapList.Add(new MemEntry(0x04600010, 0x04600013, PI_STATUS_REG_R, PI_STATUS_REG_W,               "PI_STATUS_REG"));
            MemoryMapList.Add(new MemEntry(0x04600014, 0x04600017, PI_BSD_DOM1_LAT_REG_RW, PI_BSD_DOM1_LAT_REG_RW, "PI_BSD_DOM1_LAT_REG"));
            MemoryMapList.Add(new MemEntry(0x04600018, 0x0460001B, PI_BSD_DOM1_PWD_REG_RW, PI_BSD_DOM1_PWD_REG_RW, "PI_BSD_DOM1_PWD_REG"));
            MemoryMapList.Add(new MemEntry(0x0460001C, 0x0460001F, PI_BSD_DOM1_PGS_REG_RW, PI_BSD_DOM1_PGS_REG_RW, "PI_BSD_DOM1_PGS_REG"));
            MemoryMapList.Add(new MemEntry(0x04600020, 0x04600023, PI_BSD_DOM1_RLS_REG_RW, PI_BSD_DOM1_RLS_REG_RW, "PI_BSD_DOM1_RLS_REG"));

            // SI Registers
            MemoryMapList.Add(new MemEntry(0x04800018, 0x0480001B, SI_STATUS_REG_R, SI_STATUS_REG_W, "SI_STATUS_REG"));

            // RI Registers
            MemoryMapList.Add(new MemEntry(0x0470000C, 0x0470000F, RI_SELECT_REG_RW, RI_SELECT_REG_RW, "RI_SELECT_REG"));

            // Rom
            MemoryMapList.Add(new MemEntry(0x10000000, 0x1F39FFFF, Rom, Rom, "Cartridge Domain 1 (Address 2)"));

            // PIF
            MemoryMapList.Add(new MemEntry(0x1FC00000, 0x1FC007BF, PIFROM, PIFROM, "PIF Rom"));
            MemoryMapList.Add(new MemEntry(0x1FC007C0, 0x1FC007FF, PIFRAM, PIFRAM, "PIF Ram"));

            MemoryMap = MemoryMapList.ToArray();
            MemoryMapList.Clear();

            // Setup Environment

            // MI Registers
            WriteUInt32(0x04300004, 0x02020102); // MI_VERSION_REG (Same value as Pj64 1.4)

            // VI Registers
            WriteUInt32(0x0440000C, 1023); // VI_INTR_REG

            // PI Registers
            uint BSD_DOM1_CONFIG = ReadUInt32(0x10000000);

            WriteUInt32(0x04600014, (BSD_DOM1_CONFIG      ) & 0xFF); // PI_BSD_DOM1_LAT_REG
            WriteUInt32(0x04600018, (BSD_DOM1_CONFIG >> 8 ) & 0xFF); // PI_BSD_DOM1_PWD_REG
            WriteUInt32(0x0460001C, (BSD_DOM1_CONFIG >> 16) & 0x0F); // PI_BSD_DOM1_PGS_REG
            WriteUInt32(0x04600020, (BSD_DOM1_CONFIG >> 20) & 0x03); // PI_BSD_DOM1_RLS_REG

            SP_STATUS_REG_R[3] = 0x1;

            // RI Registers
            WriteUInt32(0x0470000C, 0b1110); // RI_SELECT_REG

            // Copy the Boot Code to SP_DMEM
            FastMemoryCopy(0x04000040, 0x10000040, 0xFC0);

            // Required by CIC x105
            WriteUInt32(0x40001000, 0x3C0DBFC0);
            WriteUInt32(0x40001004, 0x8DA807FC);
            WriteUInt32(0x40001008, 0x25AD07C0);
            WriteUInt32(0x40001010, 0x5500FFFC);
            WriteUInt32(0x40001018, 0x8DA80024);
            WriteUInt32(0x4000101C, 0x3C0BB000);
        }

        public void PI_WR_LEN_WRITE_EVENT()
        {
            uint WriteLength = ReadUInt32(0x0460000C); // PI_WR_LEN_REG
            uint CartAddr    = ReadUInt32(0x04600004); // PI_CART_ADDR_REG
            uint DramAddr    = ReadUInt32(0x04600000); // PI_DRAM_ADDR_REG

            PI_STATUS_REG_R[3] |= 0b0001; // Set DMA Busy

            SafeMemoryCopy(DramAddr, CartAddr, (int)WriteLength + 1);

            PI_STATUS_REG_R[3] &= 0b1110; // Clear DMA Busy
        }

        struct MemEntry
        {
            public uint StartAddress;
            public uint EndAddress;
            public byte[] ReadArray;
            public byte[] WriteArray;
            public string Name;

            public MemoryEvent ReadEvent;
            public MemoryEvent WriteEvent;

            public MemEntry(uint StartAddress, uint EndAddress, byte[] ReadArray, byte[] WriteArray, string Name, MemoryEvent ReadEvent = null, MemoryEvent WriteEvent = null)
            {
                this.StartAddress = StartAddress;
                this.EndAddress   = EndAddress;
                this.ReadArray    = ReadArray;
                this.WriteArray   = WriteArray;
                this.Name         = Name;
                this.ReadEvent    = ReadEvent;
                this.WriteEvent   = WriteEvent;
            }
        }

        private MemEntry GetEntry(uint index)
        {
            bool FoundEntry = false;
            MemEntry Result = new MemEntry();

            for (int i = 0; i < MemoryMap.Length; ++i)
            {
                MemEntry CurrentEntry = MemoryMap[i];
                if (index < CurrentEntry.StartAddress || index > CurrentEntry.EndAddress) continue;

                FoundEntry = true;
                Result = CurrentEntry;
                break;
            }

            if (!FoundEntry)
            {
                throw new Common.Exceptions.InvalidOrUnimplementedMemoryMapException($"\"0x{index:x8}\" does not pertain to any mapped memory." +
                    $"  PC: 0x{Registers.R4300.PC:x8}");
            }

            return Result;
        }

        public byte this[uint index]
        {
            get
            {
                uint nonCachedIndex = TLB.TranslateAddress(index) & 0x1FFFFFFF;
                MemEntry Entry = GetEntry(nonCachedIndex);

                if (Entry.ReadArray == null)
                    throw new Common.Exceptions.MemoryProtectionViolation($"Memory at \"0x{index:x8}\" is not readable.");

                return Entry.ReadArray[nonCachedIndex - Entry.StartAddress];
            }
            set
            {
                uint nonCachedIndex = TLB.TranslateAddress(index) & 0x1FFFFFFF;
                MemEntry Entry = GetEntry(nonCachedIndex);

                if (Entry.WriteArray == null)
                    throw new Common.Exceptions.MemoryProtectionViolation($"Memory at \"0x{index:x8}\" is not writable.");

                Entry.WriteArray[nonCachedIndex - Entry.StartAddress] = value;
            }
        }

        public byte[] this[uint index, int size]
        {
            get
            {
                uint nonCachedIndex = TLB.TranslateAddress(index) & 0x1FFFFFFF;
                byte[] result = new byte[size];
                MemEntry Entry = GetEntry(nonCachedIndex);

                if (Entry.ReadArray == null)
                    throw new Common.Exceptions.MemoryProtectionViolation($"Memory at \"0x{index:x8}\" is not readable.");

                Buffer.BlockCopy(Entry.ReadArray, (int)(nonCachedIndex - Entry.StartAddress), result, 0, size);

                Entry.ReadEvent?.Invoke();

                return result;
            }
            set
            {
                uint nonCachedIndex = TLB.TranslateAddress(index) & 0x1FFFFFFF;
                MemEntry Entry = GetEntry(nonCachedIndex);

                if (Entry.WriteArray == null)
                    throw new Common.Exceptions.MemoryProtectionViolation($"Memory at \"0x{index:x8}\" is not writable.");

                Buffer.BlockCopy(value, 0, Entry.WriteArray, (int)(nonCachedIndex - Entry.StartAddress), size);

                Entry.WriteEvent?.Invoke();
            }
        }

        public void FastMemoryWrite(uint Dest, byte[] ToWrite)
        {
            this[Dest, ToWrite.Length] = ToWrite;
        }

        public byte[] FastMemoryRead(uint Source, int Length)
        {
            return this[Source, Length];
        }

        public void FastMemoryWrite(uint Dest, byte[] ToWrite, int Length)
        {
            if (ToWrite.Length < Length)
                throw new InvalidOperationException("Cannot write to memory an Array that is less than the input size.");
            this[Dest, Length] = ToWrite;
        }

        public void FastMemoryCopy(uint Dest, uint Source, int Size)
        {
            FastMemoryWrite(Dest, FastMemoryRead(Source, Size));
        }

        public void SafeMemoryCopy(uint Dest, uint Source, int Size)
        {
            if (GetEntry(Source & 0x1FFFFFFF).StartAddress != GetEntry((Source + (uint)Size) & 0x1FFFFFFF).StartAddress 
                || GetEntry(Dest & 0x1FFFFFFF).StartAddress != GetEntry((Dest + (uint)Size) & 0x1FFFFFFF).StartAddress)
                throw new NotImplementedException("Copying over multiple Memory Regions isn't implemented.");
            FastMemoryCopy(Dest, Source, Size);
        }

        public byte ReadUInt8(uint index)
        {
            return this[index];
        }

        public void WriteUInt8(uint index, byte value)
        {
            this[index] = value;
        }

        public sbyte ReadInt8(uint index)
        {
            return (sbyte)ReadUInt8(index);
        }

        public void WriteInt8(uint index, sbyte value)
        {
            WriteUInt8(index, (byte)value);
        }

        public ushort ReadUInt16(uint index)
        {
            byte[] Res = this[index, 2];
            Array.Reverse(Res);
            unsafe
            {
                fixed (byte* point = &Res[0])
                {
                    ushort* shortPoint = (ushort*)point;
                    return *shortPoint;
                }
            }
        }

        public void WriteUInt16(uint index, ushort value)
        {
            unsafe
            {
                ushort* point = &value;
                byte[] PointArray = new byte[2];
                Marshal.Copy(new IntPtr(point), PointArray, 0, 2);

                Array.Reverse(PointArray);

                this[index, 2] = PointArray;
            }
        }

        public short ReadInt16(uint index)
        {
            return (short)ReadUInt16(index);
        }

        public void WriteInt16(uint index, short value)
        {
            WriteUInt16(index, (ushort)value);
        }

        public uint ReadUInt32(uint index)
        {
            byte[] Res = this[index, 4];
            Array.Reverse(Res);
            unsafe
            {
                fixed (byte* point = &Res[0])
                {
                    uint* intPoint = (uint*)point;
                    return *intPoint;
                }
            }
        }

        public void WriteUInt32(uint index, uint value)
        {
            unsafe
            {
                uint* point = &value;
                byte[] PointArray = new byte[4];
                Marshal.Copy(new IntPtr(point), PointArray, 0, 4);

                Array.Reverse(PointArray);

                this[index, 4] = PointArray;
            }
        }

        public int ReadInt32(uint index)
        {
            return (int)ReadUInt32(index);
        }

        public void WriteInt32(uint index, int value)
        {
            WriteUInt32(index, (uint)value);
        }

        public ulong ReadUInt64(uint index)
        {
            byte[] Res = this[index, 8];
            Array.Reverse(Res);
            unsafe
            {
                fixed (byte* point = &Res[0])
                {
                    ulong* longPoint = (ulong*)point;
                    return *longPoint;
                }
            }
        }

        public void WriteUInt64(uint index, ulong value)
        {
            unsafe
            {
                ulong* point = &value;
                byte[] PointArray = new byte[8];
                Marshal.Copy(new IntPtr(point), PointArray, 0, 8);

                Array.Reverse(PointArray);

                this[index, 8] = PointArray;
            }
        }

        public long ReadInt64(uint index)
        {
            return (long)ReadUInt64(index);
        }

        public void WriteInt64(uint index, long value)
        {
            WriteUInt64(index, (ulong)value);
        }
    }
}
