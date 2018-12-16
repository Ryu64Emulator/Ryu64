using System;

namespace Ryu64.Common
{
    public class Util
    {
        public static void Cleanup(uint PC)
        {
            Measure.MeasureTime.Stop();
            double MHz = Measure.CycleCounter / (1000000 * Measure.MeasureTime.Elapsed.TotalSeconds);
            Logger.PrintSuccessLine($"Took {Measure.MeasureTime.Elapsed:c}, Instructions Executed: {Measure.InstructionCount}, Cycles Counted: {Measure.CycleCounter}, Estimated MHz: {MHz}, stopped at 0x{PC:x8}.");
            Logger.PrintSuccessLine($"Total memory allocated: {GetByteSizeString(GC.GetTotalMemory(true))}");
        }

        public static string GetByteSizeString(decimal Bytes)
        {
            if (Bytes < 1024)
            {
                return $"{Bytes:#,#} B(s)";
            }
            else if (Bytes < 1048576)
            {
                return $"{Bytes / 1024:#,#} KB(s)";
            }
            else if (Bytes < 1073741824)
            {
                return $"{Bytes / 1048576:#,#} MB(s)";
            }
            else if (Bytes < 1099511627776)
            {
                return $"{Bytes / 1073741824:#,#} GB(s)";
            }

            return null;
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
