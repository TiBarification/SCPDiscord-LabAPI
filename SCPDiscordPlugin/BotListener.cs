using SCPDiscord.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using PluginAPI.Core;
using PluginAPI.Helpers;

namespace SCPDiscord
{
	class BotListener
	{
		private readonly SCPDiscord plugin;
		public BotListener(SCPDiscord plugin)
		{
			this.plugin = plugin;
			while (true)
			{
				try
				{
					//Listen for connections
					if (NetworkSystem.IsConnected())
					{
						MessageWrapper data;
						try
						{
							data = MessageWrapper.Parser.ParseDelimitedFrom(NetworkSystem.networkStream);
						}
						catch (Exception e)
						{
							if (e is IOException)
								plugin.Error("Connection to bot lost.");

							else
								plugin.Error("Couldn't parse incoming packet!\n" + e);
							return;
						}

						plugin.Debug("Incoming packet: " + Google.Protobuf.JsonFormatter.Default.Format(data));

						switch (data.MessageCase)
						{
							case MessageWrapper.MessageOneofCase.SyncRoleCommand:
								plugin.SendEmbedByID(plugin.roleSync.AddPlayer(data.SyncRoleCommand.SteamIDOrIP, data.SyncRoleCommand.DiscordID, data.SyncRoleCommand.ChannelID));
								break;

							case MessageWrapper.MessageOneofCase.UnsyncRoleCommand:
								plugin.SendEmbedByID(plugin.roleSync.RemovePlayer(data.UnsyncRoleCommand.DiscordID, data.UnsyncRoleCommand.ChannelID));
								break;

							case MessageWrapper.MessageOneofCase.ConsoleCommand:
								plugin.sync.ScheduleDiscordCommand(data.ConsoleCommand);
								break;

							case MessageWrapper.MessageOneofCase.UserInfo:
								plugin.roleSync.ReceiveQueryResponse(data.UserInfo);
								break;

							case MessageWrapper.MessageOneofCase.BanCommand:
								BanCommand(data.BanCommand.ChannelID, data.BanCommand.SteamID, data.BanCommand.Duration, data.BanCommand.Reason, data.BanCommand.AdminTag);
								break;

							case MessageWrapper.MessageOneofCase.UnbanCommand:
								UnbanCommand(data.UnbanCommand.ChannelID, data.UnbanCommand.SteamIDOrIP);
								break;

							case MessageWrapper.MessageOneofCase.KickCommand:
								KickCommand(data.KickCommand.ChannelID, data.KickCommand.SteamID, data.KickCommand.Reason, data.KickCommand.AdminTag);
								break;

							case MessageWrapper.MessageOneofCase.KickallCommand:
								KickAllCommand(data.KickallCommand.ChannelID, data.KickallCommand.Reason, data.KickallCommand.AdminTag);
								break;

							case MessageWrapper.MessageOneofCase.ListCommand:
								var reply = "```md\n# Players online (" + (Player.Count - 1) + "):\n";
								foreach (Player player in Player.GetPlayers<Player>())
								{
									reply += player.Nickname.PadRight(35) + "<" + player.UserId + ">" + "\n";
								}
								reply += "```";
								plugin.SendStringByID(data.ListCommand.ChannelID, reply);
								break;

							case MessageWrapper.MessageOneofCase.BotActivity:
							case MessageWrapper.MessageOneofCase.ChatMessage:
							case MessageWrapper.MessageOneofCase.UserQuery:
								plugin.Warn("Received packet meant for bot: " + Google.Protobuf.JsonFormatter.Default.Format(data));
								break;

							case MessageWrapper.MessageOneofCase.None:
							default:
								plugin.Warn("Unknown packet received: " + Google.Protobuf.JsonFormatter.Default.Format(data));
								break;
						}
					}
					Thread.Sleep(1000);
				}
				catch (Exception ex)
				{
					plugin.Error("BotListener Error: " + ex);
				}
			}
		}

		private void BanCommand(ulong channelID, string steamID, string duration, string reason, string adminTag)
		{
			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Red,
				ChannelID = channelID
			};

			// Perform very basic SteamID validation.
			if (!IsPossibleSteamID(steamID))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamid", steamID }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamid", variables);
				return;
			}

			// Create duration timestamp.
			string humanReadableDuration = "";
			DateTime endTime;
			try
			{
				endTime = ParseBanDuration(duration, ref humanReadableDuration);
			}
			catch (IndexOutOfRangeException)
			{
				endTime = DateTime.MinValue;
			}

			if (endTime == DateTime.MinValue)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration", duration }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidduration", variables);
				return;
			}

			string name = "";
			if (!plugin.GetPlayerName(steamID, ref name))
			{
				name = "Offline player";
			}

			//Semicolons are separators in the ban file so cannot be part of strings
			name = name.Replace(";", "");
			reason = reason.Replace(";", "");

			if (reason == "")
			{
				reason = "No reason provided.";
			}

			// Add the player to the SteamIDBans file.
			StreamWriter streamWriter = new StreamWriter(Config.GetUserIDBansFile(), true);
			streamWriter.WriteLine(name + ';' + (steamID.EndsWith("@steam") ? steamID : steamID + "@steam") + ';' + endTime.Ticks + ';' + reason + ";" + adminTag + ";" + DateTime.UtcNow.Ticks);
			streamWriter.Dispose();

			// Kicks the player if they are online.
			plugin.KickPlayer(steamID, "Banned for the following reason: '" + reason + "'");

			Dictionary<string, string> banVars = new Dictionary<string, string>
			{
				{ "name",       name                    },
				{ "steamid",    steamID                 },
				{ "reason",     reason                  },
				{ "duration",   humanReadableDuration   },
				{ "admintag",   adminTag                }
			};

			embed.Colour = EmbedMessage.Types.DiscordColour.Green;
			plugin.SendEmbedWithMessageByID(embed, "messages.playerbanned", banVars);
		}

		private void UnbanCommand(ulong channelID, string steamIDOrIP)
		{
			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Red,
				ChannelID = channelID
			};

			// Perform very basic SteamID and ip validation.
			if (!IsPossibleSteamID(steamIDOrIP) && !IPAddress.TryParse(steamIDOrIP, out IPAddress _))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamidorip", steamIDOrIP }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamidorip", variables);
				return;
			}

			// Read ip bans if the file exists
			List<string> ipBans = new List<string>();
			if (File.Exists(Config.GetIPBansFile()))
			{
				 ipBans = File.ReadAllLines(Config.GetIPBansFile()).ToList();
			}
			else
			{
				plugin.Warn(Config.GetIPBansFile() + " does not exist, could not check it for banned players.");
			}

			// Read steam id bans if the file exists
			List<string> steamIDBans = new List<string>();
			if (File.Exists(Config.GetUserIDBansFile()))
			{
				steamIDBans = File.ReadAllLines(Config.GetUserIDBansFile()).ToList();
			}
			else
			{
				plugin.Warn(Config.GetUserIDBansFile() + " does not exist, could not check it for banned players.");
			}

			// Get all ban entries to be removed. (Splits the string and only checks the steam id and ip of the banned players instead of entire strings)
			List<string> matchingIPBans = ipBans.FindAll(s => s.Split(';').ElementAtOrDefault(1)?.Contains(steamIDOrIP) ?? false);
			List<string> matchingSteamIDBans = steamIDBans.FindAll(s => s.Split(';').ElementAtOrDefault(1)?.Contains(steamIDOrIP) ?? false);

			// Delete the entries from the original containers now that there is a backup of them
			ipBans.RemoveAll(s => matchingIPBans.Any(str => str == s));
			steamIDBans.RemoveAll(s => matchingSteamIDBans.Any(str => str == s));

			// Check if either ban file has a ban with a time stamp matching the one removed and remove it too as
			// most servers create both a steamid-ban entry and an ip-ban entry.
			foreach (var row in matchingIPBans)
			{
				steamIDBans.RemoveAll(s => s.Contains(row.Split(';').Last()));
			}

			foreach (var row in matchingSteamIDBans)
			{
				ipBans.RemoveAll(s => s.Contains(row.Split(';').Last()));
			}

			// Save the edited ban files if they exist
			if (File.Exists(Config.GetIPBansFile()))
			{
				File.WriteAllLines(Config.GetIPBansFile(), ipBans);
			}
			if (File.Exists(Config.GetUserIDBansFile()))
			{
				File.WriteAllLines(Config.GetUserIDBansFile(), steamIDBans);
			}

			// Send response message to Discord
			Dictionary<string, string> unbanVars = new Dictionary<string, string>
			{
				{ "steamidorip", steamIDOrIP }
			};
			embed.Colour = EmbedMessage.Types.DiscordColour.Green;
			plugin.SendEmbedWithMessageByID(embed, "messages.playerunbanned", unbanVars);
		}

		private void KickCommand(ulong channelID, string steamID, string reason, string adminTag)
		{
			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Red,
				ChannelID = channelID
			};

			//Perform very basic SteamID validation
			if (!IsPossibleSteamID(steamID))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamid", steamID }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamid", variables);
				return;
			}

			//Get player name for feedback message
			string playerName = "";
			plugin.GetPlayerName(steamID, ref playerName);

			//Kicks the player
			if (plugin.KickPlayer(steamID, reason))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "name", playerName },
					{ "steamid", steamID },
					{ "admintag", adminTag }
				};
				embed.Colour = EmbedMessage.Types.DiscordColour.Green;
				plugin.SendEmbedWithMessageByID(embed, "messages.playerkicked", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamid", steamID }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.playernotfound", variables);
			}
		}

		private void KickAllCommand(ulong channelID, string reason, string adminTag)
		{
			if (reason == "")
			{
				reason = "All players kicked by Admin";
			}
			foreach (Player player in Player.GetPlayers<Player>())
			{
				player.Ban(reason, 0);
			}
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "reason", reason },
				{ "admintag", adminTag}
			};

			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Green,
				ChannelID = channelID
			};
			plugin.SendEmbedWithMessageByID(embed, "messages.kickall", variables);
		}

		private static DateTime ParseBanDuration(string duration, ref string humanReadableDuration)
		{
			//Check if the amount is a number
			if (!int.TryParse(new string(duration.Where(char.IsDigit).ToArray()), out int amount))
			{
				return DateTime.MinValue;
			}

			char unit = duration.Where(char.IsLetter).ToArray()[0];
			TimeSpan timeSpanDuration = new TimeSpan();

			// Parse time into a TimeSpan duration and string
			if (unit == 's')
			{
				humanReadableDuration = amount + " second";
				timeSpanDuration = new TimeSpan(0, 0, 0, amount);
			}
			else if (unit == 'm')
			{
				humanReadableDuration = amount + " minute";
				timeSpanDuration = new TimeSpan(0, 0, amount, 0);
			}
			else if (unit == 'h')
			{
				humanReadableDuration = amount + " hour";
				timeSpanDuration = new TimeSpan(0, amount, 0, 0);
			}
			else if (unit == 'd')
			{
				humanReadableDuration = amount + " day";
				timeSpanDuration = new TimeSpan(amount, 0, 0, 0);
			}
			else if (unit == 'w')
			{
				humanReadableDuration = amount + " week";
				timeSpanDuration = new TimeSpan(7 * amount, 0, 0, 0);
			}
			else if (unit == 'M')
			{
				humanReadableDuration = amount + " month";
				timeSpanDuration = new TimeSpan(30 * amount, 0, 0, 0);
			}
			else if (unit == 'y')
			{
				humanReadableDuration = amount + " year";
				timeSpanDuration = new TimeSpan(365 * amount, 0, 0, 0);
			}

			// Pluralize string if needed
			if (amount != 1)
			{
				humanReadableDuration += 's';
			}

			return DateTime.UtcNow.Add(timeSpanDuration);
		}

		private static bool IsPossibleSteamID(string steamID)
		{
			return steamID.Length >= 17 && ulong.TryParse(steamID.Replace("@steam", ""), out ulong _);
		}
	}
}
