using System.Configuration;

namespace JobQueueCore
{
    public static class JobConfiguration
    {
        public static ApplicationSettingsBase AppSettings { get; set; }

        public static bool LogDebugInfo()
        {
            const string s = "LogDebugInfo";
            foreach (SettingsProperty property in AppSettings.Properties)
            {
                if (property.Name == s)
                    return (AppSettings[s].ToString().ToLower() == "true");
            }

            return false;
        }
    }
}
