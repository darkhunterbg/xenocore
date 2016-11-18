using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Builder.Services
{
    public static class ConfigurationService
    {
        private static Configuration configuration;

        public static void Initialize()
        {
            var path = Utilities.GetExecutablePath();
            configuration = ConfigurationManager.OpenExeConfiguration(path);
        }

        private static String Read(String key)
        {
            return configuration.AppSettings.Settings[key]?.Value ?? String.Empty;
        }
        private static void Write(String key, String value)
        {
            var entry = configuration.AppSettings.Settings[key];
            if (entry == null)
                configuration.AppSettings.Settings.Add(key, value);
            else
                entry.Value = value;

            configuration.Save(ConfigurationSaveMode.Minimal);
        }

        public static String ContentDir
        {
            get
            {
                return Read("contentDir");
            }
            set
            {
                Write("contentDir", value);
            }
        }

        public static void Uninitialize()
        {
            configuration.Save(ConfigurationSaveMode.Full);
        }
    }
}
