using Newtonsoft.Json;
using SCPDiscord.Commands;
using SCPDiscord.EventListeners;
using SCPDiscord.Interface;
using SCPDiscord.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Mirror.LiteNetLib4Mirror;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using YamlDotNet.Core;

namespace SCPDiscord
{
	public class SCPDiscord
	{
		public readonly Stopwatch serverStartTime = new Stopwatch();

		internal SynchronousExecutor sync;

		internal static SCPDiscord plugin;

		public bool roundStarted = false;

		public RoleSync roleSync;

		public bool shutdown;

		public const string VERSION = "3.0.0-alpha1";

		[PluginEntryPoint("SCPDiscord", VERSION, "SCP:SL - Discord bridge.", "Karl Essinger")]
		public void Start()
		{
			plugin = this;

			serverStartTime.Start();

			LiteNetLib4MirrorNetworkManager.singleton.gameObject.AddComponent<SynchronousExecutor>();
			sync = LiteNetLib4MirrorNetworkManager.singleton.gameObject.GetComponent<SynchronousExecutor>();

			AddConfig(new Smod2.Config.ConfigSetting("scpdiscord_config_global", false, true, "Whether or not the config should be placed in the global config directory."));
			AddConfig(new Smod2.Config.ConfigSetting("scpdiscord_rolesync_global", true, true, "Whether or not the rolesync file should be placed in the global config directory."));
			AddConfig(new Smod2.Config.ConfigSetting("scpdiscord_languages_global", true, true, "Whether or not the languages should be placed in the global config directory."));

			// Event handlers
            EventManager.RegisterEvents(this, sync);
			EventManager.RegisterEvents<SyncPlayerRole>(this);
			EventManager.RegisterEvents<RoundEventListener>(this);
			EventManager.RegisterEvents<PlayerEventListener>(this);
			EventManager.RegisterEvents<AdminEventListener>(this);
			EventManager.RegisterEvents<EnvironmentEventListener>(this);
			EventManager.RegisterEvents<TeamEventListener>(this);
			
			SetUpFileSystem();
			roleSync = new RoleSync(this);
			LoadConfig();
			if (Server.Port == Config.GetInt("bot.port"))
			{
				Error("ERROR: Server is running on the same port as the plugin, aborting...");
				throw new Exception();
			}
			Language.Reload();

			new Thread(() => new StartNetworkSystem(plugin)).Start();

			Info("SCPDiscord " + VERSION + " enabled.");
		}

		private class SyncPlayerRole
		{
			[PluginEvent(ServerEventType.PlayerJoined)]
			public void OnPlayerJoin(Player player)
			{
				plugin.roleSync.SendRoleQuery(player);
			}
		}

		/// <summary>
		/// Makes sure all plugin files exist.
		/// </summary>
		public void SetUpFileSystem()
		{
			if (!Directory.Exists(Config.GetConfigDir()))
			{
				Directory.CreateDirectory(Config.GetConfigDir());
			}

			if (!Directory.Exists(Config.GetLanguageDir()))
			{
				Directory.CreateDirectory(Config.GetLanguageDir());
			}

			if (!Directory.Exists(Config.GetRolesyncDir()))
			{
				Directory.CreateDirectory(Config.GetRolesyncDir());
			}

			if (!File.Exists(Config.GetConfigPath()))
			{
				Info("Config file '" + Config.GetConfigPath() + "' does not exist, creating...");
				File.WriteAllText(Config.GetConfigPath(), Encoding.UTF8.GetString(Resources.config));
			}

			if (!File.Exists(Config.GetRolesyncPath()))
			{
				Info("Config file rolesync.json does not exist, creating...");
				File.WriteAllText(Config.GetConfigPath(), "[]");
			}
		}

		/// <summary>
		/// Loads all config options from the plugin config file.
		/// </summary>
		public void LoadConfig()
		{
			try
			{
				Config.Reload(plugin);
				Info("Successfully loaded config '" + Config.GetConfigPath() + "'.");
			}
			catch (Exception e)
			{
				if (e is DirectoryNotFoundException)
				{
					Error("Config directory not found.");
				}
				else if (e is UnauthorizedAccessException)
				{
					Error("Primary language file access denied.");
				}
				else if (e is FileNotFoundException)
				{
					Error("'" + Config.GetConfigPath() + "' was not found.");
				}
				else if (e is JsonReaderException || e is YamlException)
				{
					Error("'" + Config.GetConfigPath() + "' formatting error.");
				}
				Error("Error reading config file '" + Config.GetConfigPath() + "'. Aborting startup." + e);
			}
		}

		public void OnDisable()
		{
			shutdown = true;
			NetworkSystem.Disconnect();
			Log.Info("SCPDiscord disabled.");
		}

		/// <summary>
		/// Logging functions
		/// </summary>

		public void Info(string message)
		{
			Log.Info(message);
		}

		public void Warn(string message)
		{
			Log.Warning(message);
		}

		public void Error(string message)
		{
			Log.Error(message);
		}

		public void Verbose(string message)
		{
			if (Config.GetBool("settings.verbose"))
			{
				Log.Info(message);
			}
		}

		public void VerboseWarn(string message)
		{
			if (Config.GetBool("settings.verbose"))
			{
				Log.Warning(message);
			}
		}

		public void VerboseError(string message)
		{
			if (Config.GetBool("settings.verbose"))
			{
				Log.Error(message);
			}
		}

		public new void Debug(string message)
		{
			if (Config.GetBool("settings.debug"))
			{
				Log.Info(message);
			}
		}

		public void DebugWarn(string message)
		{
			if (Config.GetBool("settings.debug"))
			{
				Log.Warning(message);
			}
		}

		public void DebugError(string message)
		{
			if (Config.GetBool("settings.debug"))
			{
				Log.Error(message);
			}
		}

		// TODO: Set message functions back to void whenever piping void functions is fixed.

		/// <summary>
		/// Enqueue a string to be sent to Discord.
		/// </summary>
		/// <param name="channelAliases">The user friendly name of the channel, set in the config.</param>
		/// <param name="message">The message to be sent.</param>
		public bool SendString(IEnumerable<string> channelAliases, string message)
		{
			foreach (string channel in channelAliases)
			{
				if (Config.GetDict("aliases").ContainsKey(channel))
				{
					MessageWrapper wrapper = new MessageWrapper
					{
						ChatMessage = new ChatMessage
						{
							ChannelID = Config.GetDict("aliases")[channel],
							Content = message
						}
					};
					NetworkSystem.QueueMessage(wrapper);
				}
			}

			return true;
		}

		public bool SendEmbed(IEnumerable<string> channelAliases, EmbedMessage message)
		{
			foreach (string channel in channelAliases)
			{
				if (Config.GetDict("aliases").ContainsKey(channel))
				{
					message.ChannelID = Config.GetDict("aliases")[channel];
					NetworkSystem.QueueMessage(new MessageWrapper { EmbedMessage = message });
				}
			}

			return true;
		}

		public bool SendStringByID(ulong channelID, string message)
		{
			MessageWrapper wrapper = new MessageWrapper
			{
				ChatMessage = new ChatMessage
				{
					ChannelID = channelID,
					Content = message
				}
			};
			NetworkSystem.QueueMessage(wrapper);
			return true;
		}

		public bool SendEmbedByID(EmbedMessage message)
		{
			NetworkSystem.QueueMessage(new MessageWrapper { EmbedMessage = message });
			return true;
		}

		/// <summary>
		/// Sends a message from the loaded language file.
		/// </summary>
		/// <param name="channelAliases">A collection of channel aliases, set in the config.</param>
		/// <param name="messagePath">The language node of the message to send.</param>
		/// <param name="variables">Variables to support in the message as key value pairs.</param>
		public bool SendMessage(IEnumerable<string> channelAliases, string messagePath, Dictionary<string, string> variables = null)
		{
			foreach (string channel in channelAliases)
			{
				if (Config.GetDict("aliases").ContainsKey(channel))
				{
					Thread messageThread = new Thread(() => new ProcessMessageAsync(Config.GetDict("aliases")[channel], messagePath, variables));
					messageThread.Start();
				}
			}

			return true;
		}

		public bool SendEmbedWithMessage(IEnumerable<string> channelAliases, string messagePath, EmbedMessage embed, Dictionary<string, string> variables = null)
		{
			foreach (string channel in channelAliases)
			{
				if (Config.GetDict("aliases").ContainsKey(channel))
				{
					// Create copy to avoid pointer issues
					EmbedMessage embedCopy = new EmbedMessage(embed)
					{
						ChannelID = Config.GetDict("aliases")[channel]
					};
					Thread messageThread = new Thread(() => new ProcessEmbedMessageAsync(embedCopy, messagePath, variables));
					messageThread.Start();
				}
			}

			return true;
		}

		/// <summary>
		/// Sends a message from the loaded language file to a specific channel by channel ID. Usually used for replies to Discord messages.
		/// </summary>
		/// <param name="channelID">The ID of the channel to send to.</param>
		/// <param name="messagePath">The language node of the message to send.</param>
		/// <param name="variables">Variables to support in the message as key value pairs.</param>
		public bool SendMessageByID(ulong channelID, string messagePath, Dictionary<string, string> variables = null)
		{
			new Thread(() => new ProcessMessageAsync(channelID, messagePath, variables)).Start();
			return true;
		}

		public bool SendEmbedWithMessageByID(EmbedMessage embed, string messagePath, Dictionary<string, string> variables = null)
		{
			new Thread(() => new ProcessEmbedMessageAsync(embed, messagePath, variables)).Start();
			return true;
		}

		/// <summary>
		/// Kicks a player by SteamID.
		/// </summary>
		/// <param name="steamID">SteamID of player to be kicked.</param>
		/// <param name="message">Message to be displayed to kicked user.</param>
		/// <returns>True if player was found, false if not.</returns>
		public bool KickPlayer(string steamID, string message = "Kicked from server")
		{
			foreach (Player player in Player.GetPlayers<Player>())
			{
				if (player.GetParsedUserID() == steamID)
				{
					player.Ban(message, 0);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets a player name by SteamID.
		/// </summary>
		/// <param name="steamID">SteamID of a player.</param>
		/// <param name="name">String that will be set to the player name.</param>
		/// <returns>True if player was found, false if not.</returns>
		public bool GetPlayerName(string steamID, ref string name)
		{
			foreach (Player player in Player.GetPlayers<Player>())
			{
				if (player.GetParsedUserID() == steamID)
				{
					name = player.Nickname;
					return true;
				}
			}
			return false;
		}
	}
}
