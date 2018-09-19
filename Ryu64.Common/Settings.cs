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
        public static bool   LOG_MEM_ACCESS;
        public static bool   LOAD_PIF;
        public static bool   MEASURE_SPEED;
        public static bool   LOG_MEM_USAGE;
        public static string PIF_ROM  = "";

        public static void Parse()
        {
            try
            {
                IniParser Parse = new IniParser("./Settings.ini");
                DEBUG          = bool.Parse(Parse.Value("DEBUG", "false"));
                LOAD_PIF       = bool.Parse(Parse.Value("LOAD_PIF", "false"));
                LOG_MEM_ACCESS = bool.Parse(Parse.Value("LOG_MEM_ACCESS", "false"));
                MEASURE_SPEED  = bool.Parse(Parse.Value("MEASURE_SPEED", "false"));
                LOG_MEM_USAGE  = bool.Parse(Parse.Value("LOG_MEM_USAGE", "false"));
            }
            catch
            {
                DEBUG          = false;
                LOAD_PIF       = false;
                LOG_MEM_ACCESS = false;
                MEASURE_SPEED  = false;
                LOG_MEM_USAGE  = true;
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
