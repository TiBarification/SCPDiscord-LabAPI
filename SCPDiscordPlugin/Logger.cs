using PluginAPI.Core;

namespace SCPDiscord
{
    internal static class Logger
    {
        internal static void Info(string message)
        {
            Log.Info(message);
        }

        internal static void Warn(string message)
        {
            Log.Warning(message);
        }

        internal static void Error(string message)
        {
            Log.Error(message);
        }

        internal static void Debug(string message)
        {
            if (Config.GetBool("settings.debug"))
            {
                Log.Debug(message);
            }
        }
    }
}