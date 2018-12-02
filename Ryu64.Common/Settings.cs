using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ryu64.Common
{
    public class Settings
    {
        public static bool STEP_MODE;
        public static bool GRAPHICS_LLE;

        public static void Parse(string SettingsFile)
        {
            try
            {
                IniParser Parse = new IniParser(SettingsFile);
                GRAPHICS_LLE = bool.Parse(Parse.Value("GRAPHICS_LLE", "false"));
                STEP_MODE    = bool.Parse(Parse.Value("STEP_MODE",    "false"));
            }
            catch
            {
                GRAPHICS_LLE = false;
                STEP_MODE    = false;
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
