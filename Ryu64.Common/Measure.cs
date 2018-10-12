using System.Diagnostics;

namespace Ryu64.Common
{
    public class Measure
    {
        public static Stopwatch MeasureTime = new Stopwatch();
        public static ulong InstructionCount;
        public static ulong CycleCounter = 0;
    }
}
