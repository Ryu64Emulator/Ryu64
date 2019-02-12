using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ryu64.MIPS
{
    public class Memory
    {
        private delegate void MemoryEvent();

        public byte[] SP_DMEM_RW          = new byte[0x1000];
        public byte[] SP_IMEM_RW          = new byte[0x1000];
        public byte[] SP_MEM_ADDR_REG_RW  = new byte[4];
        public byte[] SP_DRAM_ADDR_REG_RW = new byte[4];
        public byte[] SP_RD_LEN_REG_RW    = new byte[4];
        public byte[] SP_WR_LEN_REG_RW    = new byte[4];
        public byte[] SP_STATUS_REG_R     = new byte[4];
        public byte[] SP_STATUS_REG_W     = new byte[4];
        public byte[] SP_DMA_BUSY_REG_R   = new byte[4];
        public byte[] SP_DMA_BUSY_REG_W   = new byte[4];
        public byte[] SP_SEMAPHORE_REG_R  = new byte[4];
        public byte[] SP_SEMAPHORE_REG_W  = new byte[4];
        public byte[] SP_PC_REG_RW        = new byte[4];

        public byte[] DPC_START_REG_RW   = new byte[4];
        public byte[] DPC_END_REG_RW     = new byte[4];
        public byte[] DPC_CURRENT_REG_R  = new byte[4];
        public byte[] DPC_STATUS_REG_R   = new byte[4];
        public byte[] DPC_STATUS_REG_W   = new byte[4];
        public byte[] DPC_CLOCK_REG_R    = new byte[4];
        public byte[] DPC_BUFBUSY_REG_R  = new byte[4];
        public byte[] DPC_PIPEBUSY_REG_R = new byte[4];
        public byte[] DPC_TMEM_REG_R     = new byte[4];

        public byte[] MI_INIT_MODE_REG_R = new byte[4];
        public byte[] MI_INIT_MODE_REG_W = new byte[4];
        public byte[] MI_VERSION_REG_RW  = new byte[4];
        public byte[] MI_INTR_REG_RW     = new byte[4];
        public byte[] MI_INTR_MASK_REG_R = new byte[4];
        public byte[] MI_INTR_MASK_REG_W = new byte[4];

        public byte[] VI_STATUS_REG_RW  = new byte[4];
        public byte[] VI_ORIGIN_REG_RW  = new byte[4];
        public byte[] VI_WIDTH_REG_RW   = new byte[4];
        public byte[] VI_INTR_REG_RW    = new byte[4];
        public byte[] VI_CURRENT_REG_R  = new byte[4];
        public byte[] VI_CURRENT_REG_W  = new byte[4];
        public byte[] VI_BURST_REG_RW   = new byte[4];
        public byte[] VI_V_SYNC_REG_RW  = new byte[4];
        public byte[] VI_H_SYNC_REG_RW  = new byte[4];
        public byte[] VI_LEAP_REG_RW    = new byte[4];
        public byte[] VI_H_START_REG_RW = new byte[4];
        public byte[] VI_V_START_REG_RW = new byte[4];
        public byte[] VI_V_BURST_REG_RW = new byte[4];
        public byte[] VI_X_SCALE_REG_RW = new byte[4];
        public byte[] VI_Y_SCALE_REG_RW = new byte[4];

        public byte[] AI_DRAM_ADDR_REG_W = new byte[4];
        public byte[] AI_LEN_REG_RW      = new byte[4];
        public byte[] AI_CONTROL_REG_W   = new byte[4];
        public byte[] AI_STATUS_REG_R    = new byte[4];
        public byte[] AI_STATUS_REG_W    = new byte[4];
        public byte[] AI_DACRATE_REG_W   = new byte[4];
        public byte[] AI_BITRATE_REG_W   = new byte[4];

        public byte[] PI_DRAM_ADDR_REG_RW    = new byte[4];
        public byte[] PI_CART_ADDR_REG_RW    = new byte[4];
        public byte[] PI_WR_LEN_REG_RW       = new byte[4];
        public byte[] PI_STATUS_REG_R        = new byte[4];
        public byte[] PI_STATUS_REG_W        = new byte[4];
        public byte[] PI_BSD_DOM1_LAT_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM1_PWD_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM1_PGS_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM1_RLS_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM2_LAT_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM2_PWD_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM2_PGS_REG_RW = new byte[4];
        public byte[] PI_BSD_DOM2_RLS_REG_RW = new byte[4];

        public byte[] SI_STATUS_REG_R  = new byte[4];
        public byte[] SI_STATUS_REG_W  = new byte[4];

        public byte[] RI_SELECT_REG_RW = new byte[4];

        public byte[] RDRAM;
        public byte[] RDRAMReg  = new byte[1048576];
        public byte[] PIFROM    = new byte[1984];
        public byte[] PIFRAM    = new byte[64];

        private List<MemEntry> MemoryMapList = new List<MemEntry>();
        private MemEntry[]     MemoryMap;

        public Memory(byte[] Rom)
        {
            if (Common.Settings.EXPANSION_PAK)
                RDRAM = new byte[8388608];
            else
                RDRAM = new byte[4194304];

            // RDRAM
            MemoryMapList.Add(new MemEntry(0x00000000, 0x00000000 + (uint)(RDRAM.Length - 1), RDRAM, RDRAM, "RDRAM"));
            MemoryMapList.Add(new MemEntry(0x03F00000, 0x03FFFFFF, RDRAMReg, RDRAMReg, "RDRAM Registers"));

            // SP Registers
            MemoryMapList.Add(new MemEntry(0x04000000, 0x04000FFF, SP_DMEM_RW,          SP_DMEM_RW,          "SP_DMEM"));
            MemoryMapList.Add(new MemEntry(0x04001000, 0x04001FFF, SP_IMEM_RW,          SP_IMEM_RW,          "SP_IMEM"));
            MemoryMapList.Add(new MemEntry(0x04040000, 0x04040003, SP_MEM_ADDR_REG_RW,  SP_MEM_ADDR_REG_RW,  "SP_MEM_ADDR_REG"));
            MemoryMapList.Add(new MemEntry(0x04040004, 0x04040007, SP_DRAM_ADDR_REG_RW, SP_DRAM_ADDR_REG_RW, "SP_DRAM_ADDR_REG"));
            MemoryMapList.Add(new MemEntry(0x04040008, 0x0404000B, SP_RD_LEN_REG_RW,    SP_RD_LEN_REG_RW,    "SP_RD_LEN_REG",
                null, SP_RD_LEN_WRITE_EVENT));
            MemoryMapList.Add(new MemEntry(0x0404000C, 0x0404000F, SP_WR_LEN_REG_RW,    SP_WR_LEN_REG_RW,    "SP_WR_LEN_REG",
                null, SP_WR_LEN_WRITE_EVENT));
            MemoryMapList.Add(new MemEntry(0x04040010, 0x04040013, SP_STATUS_REG_R,     SP_STATUS_REG_W,     "SP_STATUS_REG",
                null, SP_STATUS_WRITE_EVENT));
            MemoryMapList.Add(new MemEntry(0x04040018, 0x0404001B, SP_DMA_BUSY_REG_R,   SP_DMA_BUSY_REG_W,   "SP_DMA_BUSY_REG"));
            MemoryMapList.Add(new MemEntry(0x0404001C, 0x0404001F, SP_SEMAPHORE_REG_R,  SP_SEMAPHORE_REG_W,  "SP_SEMAPHORE_REG"));
            MemoryMapList.Add(new MemEntry(0x04080000, 0x04080003, SP_PC_REG_RW,        SP_PC_REG_RW,        "SP_PC_REG"));

            // DPC Registers
            MemoryMapList.Add(new MemEntry(0x04100000, 0x04100003, DPC_START_REG_RW, DPC_START_REG_RW, "DPC_START_REG"));
            MemoryMapList.Add(new MemEntry(0x04100004, 0x04100007, DPC_END_REG_RW,   DPC_END_REG_RW,   "DPC_END_REG",
                null, DPC_END_WRITE_EVENT));
            MemoryMapList.Add(new MemEntry(0x04100008, 0x0410000B, DPC_CURRENT_REG_R,  null,             "DPC_CURRENT_REG"));
            MemoryMapList.Add(new MemEntry(0x0410000C, 0x0410000F, DPC_STATUS_REG_R,   DPC_STATUS_REG_W, "DPC_STATUS_REG"));
            MemoryMapList.Add(new MemEntry(0x04100010, 0x04100013, DPC_CLOCK_REG_R,    null,             "DPC_CLOCK_REG"));
            MemoryMapList.Add(new MemEntry(0x04100014, 0x04100017, DPC_BUFBUSY_REG_R,  null,             "DPC_BUFBUSY_REG"));
            MemoryMapList.Add(new MemEntry(0x04100018, 0x0410001B, DPC_PIPEBUSY_REG_R, null,             "DPC_PIPEBUSY_REG"));
            MemoryMapList.Add(new MemEntry(0x0410001C, 0x0410001F, DPC_TMEM_REG_R,     null,             "DPC_TMEM_REG"));

            // MI Registers
            MemoryMapList.Add(new MemEntry(0x04300000, 0x04300003, MI_INIT_MODE_REG_R, MI_INIT_MODE_REG_W, "MI_INIT_MODE_REG"));
            MemoryMapList.Add(new MemEntry(0x04300004, 0x04300007, MI_VERSION_REG_RW,  MI_VERSION_REG_RW,  "MI_VERSION_REG"));
            MemoryMapList.Add(new MemEntry(0x04300008, 0x0430000B, MI_INTR_REG_RW,     MI_INTR_REG_RW,     "MI_INTR_REG"));
            MemoryMapList.Add(new MemEntry(0x0430000C, 0x0430000F, MI_INTR_MASK_REG_R, MI_INTR_MASK_REG_W, "MI_INTR_MASK_REG", 
                null, MI_INTR_MASK_WRITE_EVENT));

            // VI Registers
            MemoryMapList.Add(new MemEntry(0x04400000, 0x04400003, VI_STATUS_REG_RW,  VI_STATUS_REG_RW, "VI_STATUS_REG"));
            MemoryMapList.Add(new MemEntry(0x04400004, 0x04400007, VI_ORIGIN_REG_RW,  VI_ORIGIN_REG_RW, "VI_ORIGIN_REG"));
            MemoryMapList.Add(new MemEntry(0x04400008, 0x0440000B, VI_WIDTH_REG_RW,   VI_WIDTH_REG_RW,  "VI_WIDTH_REG"));
            MemoryMapList.Add(new MemEntry(0x0440000C, 0x0440000F, VI_INTR_REG_RW,    VI_INTR_REG_RW,   "VI_INTR_REG"));
            MemoryMapList.Add(new MemEntry(0x04400010, 0x04400013, VI_CURRENT_REG_R,  VI_CURRENT_REG_W, "VI_CURRENT_REG",
                null, VI_CURRENT_WRITE_EVENT));
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
            MemoryMapList.Add(new MemEntry(0x04600024, 0x04600027, PI_BSD_DOM2_LAT_REG_RW, PI_BSD_DOM2_LAT_REG_RW, "PI_BSD_DOM2_LAT_REG"));
            MemoryMapList.Add(new MemEntry(0x04600028, 0x0460002B, PI_BSD_DOM2_PWD_REG_RW, PI_BSD_DOM2_PWD_REG_RW, "PI_BSD_DOM2_PWD_REG"));
            MemoryMapList.Add(new MemEntry(0x0460002C, 0x0460002F, PI_BSD_DOM2_PGS_REG_RW, PI_BSD_DOM2_PGS_REG_RW, "PI_BSD_DOM2_PGS_REG"));
            MemoryMapList.Add(new MemEntry(0x04600030, 0x04600033, PI_BSD_DOM2_RLS_REG_RW, PI_BSD_DOM2_RLS_REG_RW, "PI_BSD_DOM2_RLS_REG"));

            // RI Registers
            MemoryMapList.Add(new MemEntry(0x0470000C, 0x0470000F, RI_SELECT_REG_RW, RI_SELECT_REG_RW, "RI_SELECT_REG"));

            // SI Registers
            MemoryMapList.Add(new MemEntry(0x04800018, 0x0480001B, SI_STATUS_REG_R, SI_STATUS_REG_W, "SI_STATUS_REG"));

            // Rom
            if (Common.Variables.Debug) Common.Logger.PrintInfoLine($"ROM is {Common.Util.GetByteSizeString(Rom.Length)} big.");

            MemoryMapList.Add(new MemEntry(0x10000000, 0x10000000 + (uint)(Rom.Length - 1), Rom, Rom, "Cartridge Domain 1 (Address 2)"));

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

            // SP Registers
            SP_STATUS_REG_R[3] = 0x1; // Set the RSP status to be halted.

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

        public void SP_STATUS_WRITE_EVENT()
        {
            if ((SP_STATUS_REG_W[3] & 0b00000011) > 0) // Halt
            {
                if ((SP_STATUS_REG_W[3] & 0b00000011) == 0b10)
                {
                    SP_STATUS_REG_R[3] |= 0b000001;
                }
                else if ((SP_STATUS_REG_W[3] & 0b00000011) == 0b01)
                {
                    SP_STATUS_REG_R[3] &= ~0b000001 & 0xFF;
                }
            }

            if ((SP_STATUS_REG_W[3] & 0b00011000) > 0) // Intr
            {
                if ((SP_STATUS_REG_W[3] & 0b00011000) == 0b10000)
                {
                    MI_INTR_REG_RW[3] |= 0b000001;
                }
                else if ((SP_STATUS_REG_W[3] & 0b00011000) == 0b01000)
                {
                    MI_INTR_REG_RW[3] &= ~0b000001 & 0xFF;
                }
            }

            SP_STATUS_REG_W[0] = 0;
            SP_STATUS_REG_W[1] = 0;
            SP_STATUS_REG_W[2] = 0;
            SP_STATUS_REG_W[3] = 0;
        }

        public void DPC_END_WRITE_EVENT()
        {
            RDPWrapper.RDPThread.Interrupt();
        }

        public void PI_WR_LEN_WRITE_EVENT()
        {
            uint WriteLength = ReadUInt32(0x0460000C); // PI_WR_LEN_REG
            uint CartAddr    = ReadUInt32(0x04600004); // PI_CART_ADDR_REG
            uint DramAddr    = ReadUInt32(0x04600000); // PI_DRAM_ADDR_REG

            FastMemoryCopy(DramAddr, CartAddr, (int)((WriteLength + 1) & 0xFFFFFFFF));

            if (Common.Variables.Debug) Common.Logger.PrintInfoLine($"PIDMA: Type: Write, WriteLength: {WriteLength + 1}, CartAddr: 0x{CartAddr:X8}, DramAddr: 0x{DramAddr:X8}");
        }

        public void SP_WR_LEN_WRITE_EVENT()
        {
            uint WRLen = ReadUInt32(0x0404000C); // SP_WR_LEN_REG 
            uint WriteLength = (WRLen & 0xFFF) + 1;
            uint Count = ((WRLen & 0xFF000) >> 12) + 1;
            uint Skip = (WRLen & 0xFFF00000) >> 20;

            uint DramAddr = ReadUInt32(0x04040004); // SP_DRAM_ADDR_REG
            uint SourceAddr = 0;

            uint RSPMemAddr = ReadUInt32(0x04040000);
            string IMEMorDMEM = "DMEM";

            if ((RSPMemAddr & 0x1000) > 0)
            {
                SourceAddr = (RSPMemAddr & 0xFFF) + 0x04001000;
                IMEMorDMEM = "IMEM";
            }
            else
                SourceAddr = (RSPMemAddr & 0xFFF) + 0x04000000;

            if (Common.Variables.Debug) Common.Logger.PrintInfoLine($"SPDMA: Type: Write, WriteLength: {WriteLength}, Count: {Count}, Skip: {Skip}, SourceAddr: 0x{RSPMemAddr & 0xFFF:X8}, Memory Type: {IMEMorDMEM}, DramAddr: 0x{DramAddr:X8}");

            for (uint i = 0; i < Count; ++i)
            {
                FastMemoryCopy(DramAddr, SourceAddr, (int)WriteLength);

                DramAddr   += WriteLength + Skip;
                SourceAddr += WriteLength;
            }
        }

        public void SP_RD_LEN_WRITE_EVENT()
        {
            uint RDLen      = ReadUInt32(0x04040008); // SP_RD_LEN_REG 
            uint ReadLength = (RDLen & 0xFFF) + 1;
            uint Count      = ((RDLen & 0xFF000) >> 12) + 1;
            uint Skip       = (RDLen & 0xFFF00000) >> 20;

            uint DramAddr   = ReadUInt32(0x04040004); // SP_DRAM_ADDR_REG
            uint DestAddr   = 0;

            uint RSPMemAddr = ReadUInt32(0x04040000);
            string IMEMorDMEM = "DMEM";

            if ((RSPMemAddr & 0x1000) > 0)
            {
                DestAddr = (RSPMemAddr & 0xFFF) + 0x04001000;
                IMEMorDMEM = "IMEM";
            }
            else
                DestAddr = (RSPMemAddr & 0xFFF) + 0x04000000;

            if (Common.Variables.Debug) Common.Logger.PrintInfoLine($"SPDMA: Type: Read, ReadLength: {ReadLength}, Count: {Count}, Skip: {Skip}, DestAddr: 0x{RSPMemAddr & 0xFFF:X8}, Memory Type: {IMEMorDMEM}, DramAddr: 0x{DramAddr:X8}");

            for (uint i = 0; i < Count; ++i)
            {
                FastMemoryCopy(DestAddr, DramAddr, (int)ReadLength);

                DramAddr += ReadLength;
                DestAddr += ReadLength + Skip;
            }
        }

        public void MI_INIT_MODE_WRITE_EVENT()
        {
            if ((MI_INIT_MODE_REG_W[3] & 0x4) > 0) // Clear DP Interrupt
            {
                MI_INTR_REG_RW[3] &= ~0b100000 & 0xFF;
            }

            MI_INIT_MODE_REG_W[0] = 0;
            MI_INIT_MODE_REG_W[1] = 0;
            MI_INIT_MODE_REG_W[2] = 0;
            MI_INIT_MODE_REG_W[3] = 0;
        }

        public void MI_INTR_MASK_WRITE_EVENT()
        {
            if ((MI_INTR_MASK_REG_W[3] & 0b00000011) > 0) // SP
            {
                if ((MI_INTR_MASK_REG_W[3] & 0b00000011) == 0b10)
                {
                    MI_INTR_MASK_REG_R[3] |= 0b000001;
                }
                else if ((MI_INTR_MASK_REG_W[3] & 0b00000011) == 0b01)
                {
                    MI_INTR_MASK_REG_R[3] &= ~0b000001 & 0xFF;
                }
            }

            if ((MI_INTR_MASK_REG_W[3] & 0b00001100) > 0) // SI
            {
                if ((MI_INTR_MASK_REG_W[3] & 0b00001100) == 0b1000)
                {
                    MI_INTR_MASK_REG_R[3] |= 0b000010;
                }
                else if ((MI_INTR_MASK_REG_W[3] & 0b00001100) == 0b0100)
                {
                    MI_INTR_MASK_REG_R[3] &= ~0b000010 & 0xFF;
                }
            }

            if ((MI_INTR_MASK_REG_W[3] & 0b00110000) > 0) // AI
            {
                if ((MI_INTR_MASK_REG_W[3] & 0b00110000) == 0b100000)
                {
                    MI_INTR_MASK_REG_R[3] |= 0b000100;
                }
                else if ((MI_INTR_MASK_REG_W[3] & 0b00110000) == 0b010000)
                {
                    MI_INTR_MASK_REG_R[3] &= ~0b000100 & 0xFF;
                }
            }

            if ((MI_INTR_MASK_REG_W[3] & 0b11000000) > 0) // VI
            {
                if ((MI_INTR_MASK_REG_W[3] & 0b11000000) == 0b10000000)
                {
                    MI_INTR_MASK_REG_R[3] |= 0b001000;
                }
                else if ((MI_INTR_MASK_REG_W[3] & 0b11000000) == 0b01000000)
                {
                    MI_INTR_MASK_REG_R[3] &= ~0b001000 & 0xFF;
                }
            }

            if ((MI_INTR_MASK_REG_W[2] & 0b00000011) > 0) // PI
            {
                if ((MI_INTR_MASK_REG_W[2] & 0b00000011) == 0b10)
                {
                    MI_INTR_MASK_REG_R[3] |= 0b010000;
                }
                else if ((MI_INTR_MASK_REG_W[2] & 0b00000011) == 0b01)
                {
                    MI_INTR_MASK_REG_R[3] &= ~0b010000 & 0xFF;
                }
            }

            if ((MI_INTR_MASK_REG_W[2] & 0b00001100) > 0) // DP
            {
                if ((MI_INTR_MASK_REG_W[2] & 0b00001100) == 0b1000)
                {
                    MI_INTR_MASK_REG_R[3] |= 0b100000;
                }
                else if ((MI_INTR_MASK_REG_W[2] & 0b00001100) == 0b0100)
                {
                    MI_INTR_MASK_REG_R[3] &= ~0b100000 & 0xFF;
                }
            }

            MI_INTR_MASK_REG_W[0] = 0;
            MI_INTR_MASK_REG_W[1] = 0;
            MI_INTR_MASK_REG_W[2] = 0;
            MI_INTR_MASK_REG_W[3] = 0;
        }

        public void VI_CURRENT_WRITE_EVENT()
        {
            VI_CURRENT_REG_W[0] = 0;
            VI_CURRENT_REG_W[1] = 0;
            VI_CURRENT_REG_W[2] = 0;
            VI_CURRENT_REG_W[3] = 0;

            MI_INTR_REG_RW[3] &= ~0b001000 & 0xFF;
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

        private MemEntry GetEntry(uint index, bool Store = false, bool TLBMiss = true)
        {
            uint realAddress = index & 0x1FFFFFFF;
            for (int i = 0; i < MemoryMap.Length; ++i)
            {
                if (MemoryMap[i].EndAddress < realAddress || MemoryMap[i].StartAddress > realAddress)
                    continue;

                if (MemoryMap[i].EndAddress >= realAddress)
                    return MemoryMap[i];
            }

            if (TLBMiss && (index & 0xC0000000) != 0x80000000) ExceptionHandler.InvokeTLBMiss(index, Store);

            return new MemEntry()
            {
                StartAddress = uint.MaxValue
            };
        }

        public byte this[uint index, bool TLBMiss = true]
        {
            get
            {
                uint nonCachedIndex = TLB.TranslateAddress(index);
                MemEntry Entry = GetEntry(nonCachedIndex, false, TLBMiss);
                nonCachedIndex &= 0x1FFFFFFF;

                if (Entry.StartAddress == uint.MaxValue) return 0;

                if (Entry.ReadArray == null)
                    return 0;

                return Entry.ReadArray[nonCachedIndex - Entry.StartAddress];
            }
            set
            {
                uint nonCachedIndex = TLB.TranslateAddress(index);
                MemEntry Entry = GetEntry(nonCachedIndex, true, TLBMiss);
                nonCachedIndex &= 0x1FFFFFFF;

                if (Entry.StartAddress == uint.MaxValue) return;

                if (Entry.WriteArray == null)
                    return;

                Entry.WriteArray[nonCachedIndex - Entry.StartAddress] = value;
            }
        }

        public byte[] this[uint index, int size, bool TLBMiss = true]
        {
            get
            {
                uint nonCachedIndex = TLB.TranslateAddress(index);
                MemEntry Entry = GetEntry(nonCachedIndex, false, TLBMiss);
                nonCachedIndex &= 0x1FFFFFFF;

                byte[] result = new byte[size];

                if (Entry.StartAddress == uint.MaxValue) return result;

                if (Entry.ReadArray == null)
                    return result;

                nonCachedIndex -= Entry.StartAddress;

                if ((int)nonCachedIndex < 0) nonCachedIndex = 0;

                Buffer.BlockCopy(Entry.ReadArray, (int)nonCachedIndex, result, 0, size);

                Entry.ReadEvent?.Invoke();

                return result;
            }
            set
            {
                uint nonCachedIndex = TLB.TranslateAddress(index);
                MemEntry Entry = GetEntry(nonCachedIndex, true, TLBMiss);
                nonCachedIndex &= 0x1FFFFFFF;

                if (Entry.StartAddress == uint.MaxValue) return;

                if (Entry.WriteArray == null)
                    return;

                nonCachedIndex -= Entry.StartAddress;

                if ((int)nonCachedIndex < 0) nonCachedIndex = 0;

                Buffer.BlockCopy(value, 0, Entry.WriteArray, (int)nonCachedIndex, size);

                Entry.WriteEvent?.Invoke();
            }
        }

        public void WriteRDPPC(uint value)
        {
            unsafe
            {
                uint* point = &value;
                byte[] PointArray = new byte[4];
                Marshal.Copy(new IntPtr(point), PointArray, 0, 4);

                Array.Reverse(PointArray);

                Buffer.BlockCopy(PointArray, 0, DPC_CURRENT_REG_R, 0, 4);
            }
        }

        public void WriteScanline(uint value)
        {
            unsafe
            {
                uint* point = &value;
                byte[] PointArray = new byte[4];
                Marshal.Copy(new IntPtr(point), PointArray, 0, 4);

                Array.Reverse(PointArray);

                Buffer.BlockCopy(PointArray, 0, VI_CURRENT_REG_R, 0, 4);
            }
        }

        public uint ReadIMEMInstruction(uint index)
        {
            byte[] Res = new byte[4];
            Buffer.BlockCopy(SP_IMEM_RW, (int)index & 0xFFF, Res, 0, 4);
            return (uint)(Res[3] | (Res[2] << 8) | (Res[1] << 16) | (Res[0] << 24));
        }

        public void SetRSPBroke()
        {
            SP_STATUS_REG_R[3] |= 2;
        }

        public void InvokeMIInt(byte Bit)
        {
            unsafe
            {
                uint value = (uint)(1 << Bit) | ReadUInt32(0x04300008);
                uint* point = &value;
                byte[] PointArray = new byte[4];
                Marshal.Copy(new IntPtr(point), PointArray, 0, 4);

                Array.Reverse(PointArray);

                Buffer.BlockCopy(PointArray, 0, MI_INTR_REG_RW, 0, 4);
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

        public byte ReadUInt8(uint index, bool RSP, bool CPU)
        {
            if (CPU)
                return ReadUInt8(index);
            else if (RSP)
                return SP_DMEM_RW[index & 0xFFF];

            throw new ArgumentException("When reading a byte RSP nor CPU was true.");
        }

        public void WriteUInt8(uint index, byte value)
        {
            this[index] = value;
        }

        public void WriteUInt8(uint index, byte value, bool RSP, bool CPU)
        {
            if (CPU)
            {
                WriteUInt8(index, value);
                return;
            }
            else if (RSP)
            {
                SP_DMEM_RW[index & 0xFFF] = value;
                return;
            }

            throw new ArgumentException("When writing a byte RSP nor CPU was true.");
        }

        public sbyte ReadInt8(uint index)
        {
            return (sbyte)ReadUInt8(index);
        }

        public sbyte ReadInt8(uint index, bool RSP, bool CPU)
        {
            return (sbyte)ReadUInt8(index, RSP, CPU);
        }

        public void WriteInt8(uint index, sbyte value)
        {
            WriteUInt8(index, (byte)value);
        }

        public void WriteInt8(uint index, sbyte value, bool RSP, bool CPU)
        {
            WriteUInt8(index, (byte)value, RSP, CPU);
        }

        public ushort ReadUInt16(uint index)
        {
            byte[] Res = this[index, 2];
            return (ushort)((Res[1]) | (Res[0] << 8));
        }

        public ushort ReadUInt16(uint index, bool RSP, bool CPU)
        {
            if (CPU)
                return ReadUInt16(index);
            else if (RSP)
                return (ushort)((SP_DMEM_RW[(index & 0xFFF) + 1]) | (SP_DMEM_RW[index & 0xFFF] << 8));

            throw new ArgumentException("When reading a short RSP nor CPU was true.");
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

        public void WriteUInt16(uint index, ushort value, bool RSP, bool CPU)
        {
            if (CPU)
            {
                WriteUInt16(index, value);
                return;
            }
            else if (RSP)
            {
                unsafe
                {
                    ushort* point = &value;
                    byte[] PointArray = new byte[2];
                    Marshal.Copy(new IntPtr(point), PointArray, 0, 2);

                    Array.Reverse(PointArray);

                    Buffer.BlockCopy(PointArray, 0, SP_DMEM_RW, (int)index, 2);
                }
                return;
            }

            throw new ArgumentException("When writing a short RSP nor CPU was true.");
        }

        public short ReadInt16(uint index)
        {
            return (short)ReadUInt16(index);
        }

        public short ReadInt16(uint index, bool RSP, bool CPU)
        {
            return (short)ReadUInt16(index, RSP, CPU);
        }

        public void WriteInt16(uint index, short value)
        {
            WriteUInt16(index, (ushort)value);
        }

        public void WriteInt16(uint index, short value, bool RSP, bool CPU)
        {
            WriteUInt16(index, (ushort)value, RSP, CPU);
        }

        public uint ReadUInt32(uint index)
        {
            byte[] Res = this[index, 4];
            return (uint)(Res[3] | (Res[2] << 8) | (Res[1] << 16) | (Res[0] << 24));
        }

        public uint ReadUInt32(uint index, bool RSP, bool CPU)
        {
            if (CPU)
                return ReadUInt32(index);
            else if (RSP)
                return (uint)((SP_DMEM_RW[(index & 0xFFF) + 3]) | (SP_DMEM_RW[(index & 0xFFF) + 2] << 8) | (SP_DMEM_RW[(index & 0xFFF) + 1] << 16) | (SP_DMEM_RW[index & 0xFFF] << 24));

            throw new ArgumentException("When reading a uint RSP nor CPU was true.");
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

        public void WriteUInt32(uint index, uint value, bool RSP, bool CPU)
        {
            if (CPU)
            {
                WriteUInt32(index, value);
                return;
            }
            else if (RSP)
            {
                unsafe
                {
                    uint* point = &value;
                    byte[] PointArray = new byte[4];
                    Marshal.Copy(new IntPtr(point), PointArray, 0, 4);

                    Array.Reverse(PointArray);

                    Buffer.BlockCopy(PointArray, 0, SP_DMEM_RW, (int)index, 4);
                }
                return;
            }
        }

        public int ReadInt32(uint index)
        {
            return (int)ReadUInt32(index);
        }

        public int ReadInt32(uint index, bool RSP, bool CPU)
        {
            return (int)ReadUInt32(index, RSP, CPU);
        }

        public void WriteInt32(uint index, int value)
        {
            WriteUInt32(index, (uint)value);
        }

        public void WriteInt32(uint index, int value, bool RSP, bool CPU)
        {
            WriteUInt32(index, (uint)value, RSP, CPU);
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
