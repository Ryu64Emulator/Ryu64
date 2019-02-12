using System;

namespace Ryu64.Common
{
    public class Variables
    {
        public static bool UTEsyscall = false;
        public static bool Debug      = false;
        public static bool Pause      = false;
        public static bool Step       = false;
        public static double CPUMHz   = 0;

        public static string AppdataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}Ryu64";
    }
}
