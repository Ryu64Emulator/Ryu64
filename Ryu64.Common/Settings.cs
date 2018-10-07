using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ryu64.Common
{
    public class Settings
    {
        public static bool   DEBUG;
        public static bool   SUPPORT_CPUTEST_SYSCALL;
        public static bool   LOAD_PIF;
        public static bool   MEASURE_SPEED;
        public static bool   LOG_MEM_USAGE;
        public static bool   DUMP_MEMORY;
        public static uint   DUMP_MEMORY_START;
        public static uint   DUMP_MEMORY_END;
        public static bool   STEP_MODE;
        public static string PIF_ROM  = "";

        public static void Parse(string SettingsFile)
        {
            try
            {
                IniParser Parse = new IniParser(SettingsFile);
                DEBUG                   = bool.Parse(Parse.Value("DEBUG", "false"));
                SUPPORT_CPUTEST_SYSCALL = bool.Parse(Parse.Value("SUPPORT_CPUTEST_SYSCALL", "false"));
                LOAD_PIF                = bool.Parse(Parse.Value("LOAD_PIF", "false"));
                MEASURE_SPEED           = bool.Parse(Parse.Value("MEASURE_SPEED", "false"));
                LOG_MEM_USAGE           = bool.Parse(Parse.Value("LOG_MEM_USAGE", "false"));
                DUMP_MEMORY             = bool.Parse(Parse.Value("DUMP_MEMORY", "false"));
                DUMP_MEMORY_START       = Convert.ToUInt32(Parse.Value("DUMP_MEMORY_START", "0x0"), 16);
                DUMP_MEMORY_END         = Convert.ToUInt32(Parse.Value("DUMP_MEMORY_END", "0x0"), 16);
                STEP_MODE               = bool.Parse(Parse.Value("STEP_MODE", "false"));
            }
            catch
            {
                DEBUG                   = false;
                SUPPORT_CPUTEST_SYSCALL = false;
                LOAD_PIF                = false;
                MEASURE_SPEED           = false;
                LOG_MEM_USAGE           = false;
                DUMP_MEMORY             = false;
                STEP_MODE               = false;
                DUMP_MEMORY_START = 0x0;
                DUMP_MEMORY_END   = 0x0;
            }
        }

        // https://stackoverflow.com/a/37772571
        public class IniParser
        {
            Dictionary<string, string> values;
            public IniParser(string path)
            {
                values = File.ReadLines(path)
                    .Where(line => (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")))
                    .Select(line => line.Split(new char[] { '=' }, 2, 0))
                    .ToDictionary(parts => parts[0].Trim(), parts => parts.Length > 1 ? parts[1].Trim() : null);
            }

            public string Value(string name, string value = null)
            {
                if (values != null && values.ContainsKey(name))
                    return values[name];
                return value;
            }
        }
    }
}
