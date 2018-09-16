using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.MIPS
{
    public partial class InstInterp
    {
        public delegate void InterpretOpcode(uint Opcode);
    }
}
