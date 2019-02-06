using System.Threading;
using Ryu64.MIPS.Cores;

namespace Ryu64.MIPS
{
    public class VI
    {
        private static uint CurrentScanline = 0;

        public static void Start()
        {
            Thread VIThread = new Thread(() =>
            {
                while (R4300.R4300_ON)
                {
                    R4300.memory.WriteScanline(CurrentScanline);
                    MI.PollVIInterrupt(CurrentScanline);
                    ++CurrentScanline;
                    if (CurrentScanline >= R4300.memory.ReadUInt32(0x04400018))
                        CurrentScanline = 0;
                    Thread.Sleep(2);
                }
            })
            {
                Name = "VI"
            };
            VIThread.Start();
        }
    }
}
