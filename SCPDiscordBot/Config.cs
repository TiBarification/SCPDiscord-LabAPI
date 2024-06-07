﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SCPDiscord
{
	public struct Config
	{
		public struct Bot
		{
			public string token = "";
			public ulong serverId = 0;
			public string logLevel = "Debug";
			public string statusType = "DoNotDisturb";
			public string presenceType = "Watching";
			public string presenceText = "for server startup...";
			public bool disableCommands = false;

			public Bot() { }
		}
		public Bot bot;

		public Dictionary<ulong, string[]> permissions = new Dictionary<ulong, string[]>();

		public struct Plugin
		{
			public string address = "127.0.0.1";
			public int port = 8888;

			public Plugin() { }
		}
		public Plugin plugin;

		public Config() { }
	}

	public static class ConfigParser
	{
		public static bool loaded = false;

		public static Config config { get; private set; }

		private static string configPath = "config.yml";

		public static void LoadConfig()
		{
			if (!string.IsNullOrEmpty(SCPDiscordBot.commandLineArgs.configPath))
			{
				configPath = SCPDiscordBot.commandLineArgs.configPath;
			}

			Logger.Log("Loading config \"" + Path.GetFullPath(configPath) + "\"", LogID.CONFIG);

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

			loaded = true;
		}

		public static void PrintConfig()
		{
			Logger.Debug("######### Config #########", LogID.CONFIG);
			Logger.Debug("bot:", LogID.CONFIG);
			Logger.Debug("  token:            HIDDEN", LogID.CONFIG);
			Logger.Debug("  server-id:        " + config.bot.serverId, LogID.CONFIG);
			Logger.Debug("  log-level:        " + config.bot.logLevel, LogID.CONFIG);
			Logger.Debug("  presence-type:    " + config.bot.presenceType, LogID.CONFIG);
			Logger.Debug("  presence-text:    " + config.bot.presenceText, LogID.CONFIG);
			Logger.Debug("  disable-commands: " + config.bot.disableCommands, LogID.CONFIG);
			Logger.Debug("", LogID.CONFIG);
			Logger.Debug("permissions:", LogID.CONFIG);
			foreach (KeyValuePair<ulong, string[]> node in config.permissions)
			{
				Logger.Debug("  " + node.Key + ":", LogID.CONFIG);
				foreach (string command in node.Value)
				{
					Logger.Debug("    " + command, LogID.CONFIG);
				}
			}
			Logger.Debug("", LogID.CONFIG);
			Logger.Debug("plugin:", LogID.CONFIG);
			Logger.Debug("  address: " + config.plugin.address, LogID.CONFIG);
			Logger.Debug("  port:    " + config.plugin.port, LogID.CONFIG);
		}

		public static bool HasPermission(DiscordMember member, string command)
		{
			foreach (DiscordRole role in member.Roles)
			{
				Logger.Debug("Checking role '" + role.Id + "' for command permissions...", LogID.CONFIG);
				if (config.permissions.TryGetValue(role.Id, out string[] permissions))
				{
					Logger.Debug("Found role '" + role.Id + "' in config...", LogID.CONFIG);
					if (permissions.Any(s => Regex.IsMatch(command, "^" + s)))
					{
						Logger.Debug("Role '" + role.Id + "' has permission to run '" + command + "'.", LogID.CONFIG);
						return true;
					}
				}
			}

			Logger.Debug("Checking @everyone role...", LogID.CONFIG);
			if (config.permissions.TryGetValue(0, out string[] everyonePermissions))
			{
				Logger.Debug("Found @everyone role in config...", LogID.CONFIG);
				if (everyonePermissions.Any(s => Regex.IsMatch(command, "^" + s)))
				{
					Logger.Debug("Role @everyone has permission to run '" + command + "'.", LogID.CONFIG);
					return true;
				}
			}

			Logger.Debug("None of the user's roles have permission to run '" + command + "'.", LogID.CONFIG);
			return false;
		}
	}
}
