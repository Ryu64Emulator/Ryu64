using System;

namespace Ryu64.Common
{
    public class Variables
    {
        public static bool Step       = false;
        public static bool UTEsyscall = false;
        public static bool Debug      = false;

        public static string AppdataFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Ryu64";
    }
}
