using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ryu64.Common
{
    public class Measure
    {
        public static Stopwatch MeasureTime = new Stopwatch();
        public static ulong InstructionCount;
    }
}
