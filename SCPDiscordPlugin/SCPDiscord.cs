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

		public const string VERSION = "3.0.0-alpha3";

		[PluginEntryPoint("SCPDiscord", VERSION, "SCP:SL - Discord bridge.", "Karl Essinger")]
		public void Start()
		{
			plugin = this;

			serverStartTime.Start();

			LiteNetLib4MirrorNetworkManager.singleton.gameObject.AddComponent<SynchronousExecutor>();
			sync = LiteNetLib4MirrorNetworkManager.singleton.gameObject.GetComponent<SynchronousExecutor>();

			// Event handlers
            EventManager.RegisterEvents(this, sync);
			EventManager.RegisterEvents(this, new SyncPlayerRole());
			EventManager.RegisterEvents(this, new RoundEventListener(this));
			EventManager.RegisterEvents(this, new PlayerEventListener(this));
			EventManager.RegisterEvents(this, new AdminEventListener(this));
			EventManager.RegisterEvents(this, new EnvironmentEventListener(this));
			EventManager.RegisterEvents(this, new TeamEventListener(this));

			LoadConfig();
			roleSync = new RoleSync(this);
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
				Error("Error reading config file '" + Config.GetConfigPath() + "'. Aborting startup.\n" + e);
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

		public void Debug(string message)
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

		/// <summary>
		/// Enqueue a string to be sent to Discord.
		/// </summary>
		/// <param name="channelAliases">The user friendly name of the channel, set in the config.</param>
		/// <param name="message">The message to be sent.</param>
		public void SendString(IEnumerable<string> channelAliases, string message)
		{
			foreach (string channel in channelAliases)
			{
				if (Config.GetDict("channels").ContainsKey(channel))
				{
					MessageWrapper wrapper = new MessageWrapper
					{
						ChatMessage = new ChatMessage
						{
							ChannelID = Config.GetDict("channels")[channel],
							Content = message
						}
					};
					NetworkSystem.QueueMessage(wrapper);
				}
			}
		}

		public void SendEmbed(IEnumerable<string> channelAliases, EmbedMessage message)
		{
			foreach (string channel in channelAliases)
			{
				if (Config.GetDict("channels").ContainsKey(channel))
				{
					message.ChannelID = Config.GetDict("channels")[channel];
					NetworkSystem.QueueMessage(new MessageWrapper { EmbedMessage = message });
				}
			}
		}

		public void SendStringByID(ulong channelID, string message)
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
		}

		public void SendEmbedByID(EmbedMessage message)
		{
			NetworkSystem.QueueMessage(new MessageWrapper { EmbedMessage = message });
		}

		/// <summary>
		/// Sends a message from the loaded language file.
		/// </summary>
		/// <param name="channelAliases">A collection of channel channels, set in the config.</param>
		/// <param name="messagePath">The language node of the message to send.</param>
		/// <param name="variables">Variables to support in the message as key value pairs.</param>
		public void SendMessage(string messagePath, Dictionary<string, string> variables = null)
		{
			Thread messageThread = new Thread(() => new ProcessMessageAsync(messagePath, variables));
			messageThread.Start();
		}

		public void SendEmbedWithMessage(string messagePath, EmbedMessage embed, Dictionary<string, string> variables = null)
		{
			Thread messageThread = new Thread(() => new ProcessEmbedMessageAsync(embed, messagePath, variables));
			messageThread.Start();
		}

		/// <summary>
		/// Sends a message from the loaded language file to a specific channel by channel ID. Usually used for replies to Discord messages.
		/// </summary>
		/// <param name="channelID">The ID of the channel to send to.</param>
		/// <param name="messagePath">The language node of the message to send.</param>
		/// <param name="variables">Variables to support in the message as key value pairs.</param>
		public void SendMessageByID(ulong channelID, string messagePath, Dictionary<string, string> variables = null)
		{
			new Thread(() => new ProcessMessageByIDAsync(channelID, messagePath, variables)).Start();
		}

		public void SendEmbedWithMessageByID(EmbedMessage embed, string messagePath, Dictionary<string, string> variables = null)
		{
			new Thread(() => new ProcessEmbedMessageByIDAsync(embed, messagePath, variables)).Start();
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
