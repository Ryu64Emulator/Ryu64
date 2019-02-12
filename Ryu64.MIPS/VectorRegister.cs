using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public class VectorRegister
    {
        public byte[] Vector;

        public VectorRegister()
        {
            Vector = new byte[16];
        }

        public void LoadQuadword(byte[] Quadword)
        {
            for (uint i = 0; i < 16; ++i)
                Vector[i] = Quadword[i];
        }

        public ushort GetElement(uint Index)
        {
            return (ushort)((ushort)(Vector[Index] << 8) | Vector[Index + 1]);
        }

        public void SetElement(ushort value, uint Index)
        {
            Vector[Index]     = (byte)(value >> 8);
            Vector[Index + 1] = (byte)(value & 0xFF);
        }

        public void SubWithCarry(VectorRegister vs, VectorRegister vt, byte e)
        {
            Registers.RSPCOP2.VCO = 0;

            uint j = 0;
            for (uint i = 0; i < 8; ++i)
            {
                if ((e & 0b1111) == 0b0000) // Vector
                {
                    j = i;
                }
                else if ((e & 0b1110) == 0b0010) // Scalar Quarter of the Vector
                {
                    j = (uint)((e & 0b0001) + (i & 0b1110));
                }
                else if ((e & 0b1100) == 0b0100) // Scalar Half of the Vector
                {
                    j = (uint)((e & 0b0011) + (i & 0b1100));
                }
                else if ((e & 0b1000) == 0b1000) // Scalar Whole of the Vector
                {
                    j = (uint)(e & 0b0111);
                }

                int Result = vs.GetElement(i * 2) - vt.GetElement(j * 2);
                Registers.RSPCOP2.ACC[i] = (ushort)Result;
                SetElement((ushort)Result, i * 2);

                if (Result < 0)
                {
                    Registers.RSPCOP2.VCO |= (ushort)(1 << (int)i);
                    Registers.RSPCOP2.VCO |= (ushort)(1 << (int)i + 8);
                }
                else if (Result > 0)
                {
                    Registers.RSPCOP2.VCO |= (ushort)(1 << (int)i + 8);
                }
            }
        }
    }
}
