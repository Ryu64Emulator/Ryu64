using System;

namespace Ryu64.Common
{
    public class Variables
    {
        public const double N64CPUMHz = 93.75;

        public static bool UTEsyscall = false;
        public static bool Debug      = false;
        public static double CPUMHz   = 0;

        public static string AppdataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Ryu64";
    }
}
