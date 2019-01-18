using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.RDP
{
    public interface IRasterizer
    {
        void FillTriangle(ushort YL, ushort YM, ushort YH, uint XL, uint DxLDy, uint XH, uint DxHDy, uint XM, uint DxMDy, byte Dir);
        void FillRect(ushort XL, ushort YL, ushort XH, ushort YH);
    }
}
