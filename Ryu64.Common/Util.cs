using System;
using System.Collections.Generic;
using System.Text;

namespace Ryu64.Common
{
    public class Util
    {
        public static void Cleanup(uint PC)
        {
            if (Settings.MEASURE_SPEED)
            {
                Measure.MeasureTime.Stop();
                Logger.PrintSuccessLine($"Took {Measure.MeasureTime.Elapsed:c}, Instructions Executed: {Measure.InstructionCount}, stopped at 0x{PC:x8}.");
            }

            if (Settings.LOG_MEM_USAGE) Logger.PrintSuccessLine($"Allocated: {GC.GetTotalMemory(false) / 1024:#,#} kb");
        }
    }
}
