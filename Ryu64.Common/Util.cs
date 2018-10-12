﻿using System;

namespace Ryu64.Common
{
    public class Util
    {
        public static void Cleanup(uint PC)
        {
            if (Settings.MEASURE_SPEED)
            {
                Measure.MeasureTime.Stop();
                Logger.PrintSuccessLine($"Took {Measure.MeasureTime.Elapsed:c}, Instructions Executed: {Measure.InstructionCount}, Cycles Counted: {Measure.CycleCounter}, stopped at 0x{PC:x8}.");
            }

            if (Settings.LOG_MEM_USAGE) Logger.PrintSuccessLine($"Allocated: {GC.GetTotalMemory(false) / 1024:#,#} kb");
        }
    }
}
