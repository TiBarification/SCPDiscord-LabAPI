using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SCPDiscord
{
  public class Config
  {
    public class Bot
    {
      public string token { get; private set; } = "";
      public ulong serverId { get; private set; } = 0;
      public string logLevel { get; private set; } = "Debug";
      public string statusType { get; private set; } = "DoNotDisturb";
      public string presenceType { get; private set; } = "Watching";
      public string presenceText { get; private set; } = "for server startup...";
      public bool disableCommands { get; private set; } = false;
    }

    public Bot bot { get; private set; }

    public Dictionary<ulong, string[]> permissions { get; private set; } = new();

    public class Plugin
    {
      public string address { get; private set; } = "127.0.0.1";
      public int port { get; private set; } = 8888;
    }

    public Plugin plugin { get; private set; }
  }

  public static class ConfigParser
  {
    public static bool loaded { get; private set; } = false;

    public static Config config { get; private set; }

    private static string configPath = "config.yml";

    public static void LoadConfig()
    {
      if (!string.IsNullOrEmpty(SCPDiscordBot.commandLineArgs.configPath))
      {
        configPath = SCPDiscordBot.commandLineArgs.configPath;
      }

      Logger.Log("Loading config \"" + Path.GetFullPath(configPath) + "\"");

      // Writes default config to file if it does not already exist
      if (!File.Exists(configPath))
      {
        File.WriteAllText(configPath, Utilities.ReadManifestData("default_config.yml"));
      }

      // Reads config contents into FileStream
      FileStream stream = File.OpenRead(configPath);

      // Converts the FileStream into a YAML object
      IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
      config = deserializer.Deserialize<Config>(new StreamReader(stream));

      if (!Enum.TryParse(config.bot.logLevel, true, out LogLevel logLevel))
      {
        logLevel = LogLevel.Information;
        Logger.Warn("Log level '" + config.bot.logLevel + "' is invalid, using 'Information' instead.");
      }
      Logger.SetLogLevel(logLevel);

      loaded = true;
    }

    public static void PrintConfig()
    {
      Logger.Debug("######### Config #########");
      Logger.Debug("bot:");
      Logger.Debug("  token:            HIDDEN");
      Logger.Debug("  server-id:        " + config.bot.serverId);
      Logger.Debug("  log-level:        " + config.bot.logLevel);
      Logger.Debug("  presence-type:    " + config.bot.presenceType);
      Logger.Debug("  presence-text:    " + config.bot.presenceText);
      Logger.Debug("  disable-commands: " + config.bot.disableCommands);
      Logger.Debug("");
      Logger.Debug("permissions:");
      foreach (KeyValuePair<ulong, string[]> node in config.permissions)
      {
        Logger.Debug("  " + node.Key + ":");
        foreach (string command in node.Value)
        {
          Logger.Debug("    " + command);
        }
      }

      Logger.Debug("");
      Logger.Debug("plugin:");
      Logger.Debug("  address: " + config.plugin.address);
      Logger.Debug("  port:    " + config.plugin.port);
    }

    public static bool HasPermission(DiscordMember member, string command)
    {
      foreach (DiscordRole role in member.Roles)
      {
        Logger.Debug("Checking role '" + role.Id + "' for command permissions...");
        if (config.permissions.TryGetValue(role.Id, out string[] permissions))
        {
          Logger.Debug("Found role '" + role.Id + "' in config...");
          if (permissions.Any(s => Regex.IsMatch(command, "^" + s)))
          {
            Logger.Debug("Role '" + role.Id + "' has permission to run '" + command + "'.");
            return true;
          }
        }
      }

      Logger.Debug("Checking @everyone role...");
      if (config.permissions.TryGetValue(0, out string[] everyonePermissions))
      {
        Logger.Debug("Found @everyone role in config...");
        if (everyonePermissions.Any(s => Regex.IsMatch(command, "^" + s)))
        {
          Logger.Debug("Role @everyone has permission to run '" + command + "'.");
          return true;
        }
      }

      Logger.Debug("None of the user's roles have permission to run '" + command + "'.");
      return false;
    }
  }
}