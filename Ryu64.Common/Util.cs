using System;

namespace Ryu64.Common
{
    public class Util
    {
        public static void Cleanup(uint PC)
        {
            Measure.MeasureTime.Stop();
            Logger.PrintSuccessLine($"Took {Measure.MeasureTime.Elapsed:c}, Instructions Executed: {Measure.InstructionCount}, Cycles Counted: {Measure.CycleCounter}, stopped at 0x{PC:x8}.");
            Logger.PrintSuccessLine($"Allocated: {GC.GetTotalMemory(false) / 1024:#,#} kb");
        }

        public static unsafe ulong DoubleToUInt64(double value)
        {
            return *(ulong*)&value;
        }

        public static unsafe double UInt64ToDouble(ulong value)
        {
            return *(double*)&value;
        }

        public static unsafe uint FloatToUInt32(float value)
        {
            return *(uint*)&value;
        }

        public static unsafe float UInt32ToFloat(uint value)
        {
            return *(float*)&value;
        }
    }
}
