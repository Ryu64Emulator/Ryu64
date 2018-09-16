using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public static class Memory
    {
        private static readonly byte[] Mem = new byte[4194304];

        public static byte ReadUInt8(ulong Position)
        {
            return Mem[Position];
        }

        public static void WriteUInt8(ulong Position, byte Value)
        {
            Mem[Position] = Value;
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
