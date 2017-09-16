using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Inventory.Configuration
{
    public class AppConfiguration
    {
        private const string PREFIX = "INVENTORY:";

        public static AppConfiguration Settings
        {
            get
            {
                if (_settings==null)
                {
                    _settings = new AppConfiguration();
                    foreach (var setting in ConfigurationManager.AppSettings.AllKeys)
                    {
                        if (setting.ToUpper().StartsWith(PREFIX, StringComparison.InvariantCulture))
                        {
                            PropertyInfo pi = typeof(AppConfiguration).GetProperty(setting.ToUpper().Replace(PREFIX, ""), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(_settings, ConfigurationManager.AppSettings[setting]);
                            }
                            else if (pi.PropertyType == typeof(int))
                            {
                                pi.SetValue(_settings, int.Parse(ConfigurationManager.AppSettings[setting]));
                            }
                            else if (pi.PropertyType == typeof(bool))
                            {
                                pi.SetValue(_settings, bool.Parse(ConfigurationManager.AppSettings[setting]));
                            }
                            else if (pi.PropertyType == typeof(List<string>))
                            {
                                pi.SetValue(_settings, new List<string>(ConfigurationManager.AppSettings[setting].Split(',')));
                            }
                        }
                    }
                }
                return _settings;
            }
        }
        private static AppConfiguration _settings;

        public bool UseLocalRabbit { get; private set; }
        public string LocalRabbitIP { get; set; }

    }
}
