using SCPDiscord.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using PlayerRoles;
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
								plugin.SendEmbedByID(plugin.roleSync.AddPlayer(data.SyncRoleCommand));
								break;

							case MessageWrapper.MessageOneofCase.UnsyncRoleCommand:
								plugin.SendEmbedByID(plugin.roleSync.RemovePlayer(data.UnsyncRoleCommand));
								break;

							case MessageWrapper.MessageOneofCase.ConsoleCommand:
								plugin.sync.ScheduleDiscordCommand(data.ConsoleCommand);
								break;

							case MessageWrapper.MessageOneofCase.UserInfo:
								plugin.roleSync.ReceiveQueryResponse(data.UserInfo);
								break;

							case MessageWrapper.MessageOneofCase.BanCommand:
								Execute(data.BanCommand);
								break;

							case MessageWrapper.MessageOneofCase.UnbanCommand:
								Execute(data.UnbanCommand);
								break;

							case MessageWrapper.MessageOneofCase.KickCommand:
								Execute(data.KickCommand);
								break;

							case MessageWrapper.MessageOneofCase.KickallCommand:
								Execute(data.KickallCommand);
								break;

							case MessageWrapper.MessageOneofCase.ListCommand:
								Execute(data.ListCommand);
								break;

							case MessageWrapper.MessageOneofCase.BotActivity:
							case MessageWrapper.MessageOneofCase.ChatMessage:
							case MessageWrapper.MessageOneofCase.UserQuery:
							case MessageWrapper.MessageOneofCase.PaginatedMessage:
								plugin.Warn("Received packet meant for bot: " + Google.Protobuf.JsonFormatter.Default.Format(data));
								break;

							case MessageWrapper.MessageOneofCase.None:
							default:
								plugin.Warn("Unknown packet received: " + Google.Protobuf.JsonFormatter.Default.Format(data));
								break;
						}
					}
					Thread.Sleep(500);
				}
				catch (Exception ex)
				{
					plugin.Error("BotListener Error: " + ex);
				}
			}
		}

		private void Execute(BanCommand command)
		{
			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Red,
				ChannelID = command.ChannelID,
				InteractionID = command.InteractionID
			};

			// Perform very basic SteamID validation.
			if (!Utilities.IsPossibleSteamID(command.SteamID))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamid", command.SteamID }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamid", variables);
				return;
			}

			// Create duration timestamp.
			string humanReadableDuration = "";
			DateTime endTime;
			try
			{
				endTime = ParseBanDuration(command.Duration, ref humanReadableDuration);
			}
			catch (IndexOutOfRangeException)
			{
				endTime = DateTime.MinValue;
			}

			if (endTime == DateTime.MinValue)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration", command.Duration }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidduration", variables);
				return;
			}

			string name = "";
			if (!plugin.GetPlayerName(command.SteamID, ref name))
			{
				name = "Offline player";
			}

			//Semicolons are separators in the ban file so cannot be part of strings
			name = name.Replace(";", "");
			command.Reason = command.Reason.Replace(";", "");

			if (command.Reason == "")
			{
				command.Reason = "No reason provided.";
			}

			BanHandler.IssueBan(new BanDetails()
			{
				OriginalName = name,
				Id = (command.SteamID.EndsWith("@steam") ? command.SteamID : command.SteamID + "@steam"),
				IssuanceTime = DateTime.UtcNow.Ticks,
				Expires = endTime.Ticks,
				Reason = command.Reason,
				Issuer = command.AdminTag
			}, BanHandler.BanType.UserId);

			// Kicks the player if they are online.
			plugin.KickPlayer(command.SteamID, "You have been banned: '" + command.Reason + "'");

			Dictionary<string, string> banVars = new Dictionary<string, string>
			{
				{ "name",       name                   },
				{ "steamid",    command.SteamID        },
				{ "reason",     command.Reason         },
				{ "duration",   humanReadableDuration  },
				{ "admintag",   command.AdminTag       }
			};

			embed.Colour = EmbedMessage.Types.DiscordColour.Green;
			plugin.SendEmbedWithMessageByID(embed, "messages.playerbanned", banVars);
		}

		private void Execute(UnbanCommand command)
		{
			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Red,
				ChannelID = command.ChannelID,
				InteractionID = command.InteractionID
			};

			// Perform very basic SteamID and ip validation.
			if (Utilities.IsPossibleSteamID(command.SteamIDOrIP))
			{
				BanHandler.RemoveBan(command.SteamIDOrIP.EndsWith("@steam") ? command.SteamIDOrIP : command.SteamIDOrIP + "@steam", BanHandler.BanType.UserId, true);
			}
			else if (IPAddress.TryParse(command.SteamIDOrIP, out IPAddress _))
			{
				BanHandler.RemoveBan(command.SteamIDOrIP, BanHandler.BanType.IP, true);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamidorip", command.SteamIDOrIP }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamidorip", variables);
				return;
			}

			Dictionary<string, string> unbanVars = new Dictionary<string, string>
			{
				{ "steamidorip", command.SteamIDOrIP }
			};
			embed.Colour = EmbedMessage.Types.DiscordColour.Green;
			plugin.SendEmbedWithMessageByID(embed, "messages.playerunbanned", unbanVars);
		}

		private void Execute(KickCommand command)
		{
			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Red,
				ChannelID = command.ChannelID,
				InteractionID = command.InteractionID
			};

			//Perform very basic SteamID validation
			if (!Utilities.IsPossibleSteamID(command.SteamID))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamid", command.SteamID }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamid", variables);
				return;
			}

			//Get player name for feedback message
			string playerName = "";
			plugin.GetPlayerName(command.SteamID, ref playerName);

			//Kicks the player
			if (plugin.KickPlayer(command.SteamID, command.Reason))
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "name", playerName },
					{ "steamid", command.SteamID },
					{ "admintag", command.AdminTag }
				};
				embed.Colour = EmbedMessage.Types.DiscordColour.Green;
				plugin.SendEmbedWithMessageByID(embed, "messages.playerkicked", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "steamid", command.SteamID }
				};
				plugin.SendEmbedWithMessageByID(embed, "messages.playernotfound", variables);
			}
		}

		private void Execute(KickallCommand command)
		{
			if (command.Reason == "")
			{
				command.Reason = "All players kicked by Admin";
			}
			foreach (Player player in Player.GetPlayers<Player>())
			{
				player.Ban(command.Reason, 0);
			}
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "reason", command.Reason },
				{ "admintag", command.AdminTag}
			};

			EmbedMessage embed = new EmbedMessage
			{
				Colour = EmbedMessage.Types.DiscordColour.Green,
				ChannelID = command.ChannelID,
				InteractionID = command.InteractionID
			};
			plugin.SendEmbedWithMessageByID(embed, "messages.kickall", variables);
		}

		private void Execute(ListCommand command)
		{
			if (Player.Count == 0)
			{
				EmbedMessage embed = new EmbedMessage
				{
					Title = Language.GetProcessedMessage("messages.listtitle", new Dictionary<string, string>
					{
						{ "players",    Math.Max(0, Player.Count).ToString() },
						{ "maxplayers", Server.MaxPlayers.ToString()         }
					}),
					Description = Language.GetProcessedMessage("messages.listrow.empty", new Dictionary<string, string>()),
					Colour = EmbedMessage.Types.DiscordColour.Red,
					ChannelID = command.ChannelID,
					InteractionID = command.InteractionID
				};
				plugin.SendEmbedByID(embed);
				return;
			}

			List<string> listItems = new List<string>();
			foreach (Player player in Player.GetPlayers())
			{
				string row = Language.GetProcessedMessage("messages.listrow.default", new Dictionary<string, string>
				{
					{ "ipaddress",        player.IpAddress                         },
					{ "name",             player.Nickname                          },
					{ "playerid",         player.PlayerId.ToString()               },
					{ "steamid",          player.GetParsedUserID()                 },
					{ "class",            player.Role.ToString()                   },
					{ "team",             player.ReferenceHub.GetTeam().ToString() }
				});

				// Remove sensitive information if set in config
				if (Config.GetChannelIDs("channelsettings.filterips").Contains(command.ChannelID))
				{
					row = row.Replace(player.IpAddress, new string('#', player.IpAddress.Length));
				}
				if (Config.GetChannelIDs("channelsettings.filtersteamids").Contains(command.ChannelID))
				{
					row = row.Replace(player.GetParsedUserID(), "Player " + player.PlayerId);
				}

				listItems.Add(row);
			}

			List<EmbedMessage> embeds = new List<EmbedMessage>();
			foreach (string message in Utilities.ParseListIntoMessages(listItems))
			{
				embeds.Add(new EmbedMessage
				{
					Title = Language.GetProcessedMessage("messages.listtitle", new Dictionary<string, string>
					{
						{ "players",    Math.Max(0, Player.Count).ToString() },
						{ "maxplayers", Server.MaxPlayers.ToString()         }
					}),
					Colour = EmbedMessage.Types.DiscordColour.Cyan,
					Description = message
				});
			}

			PaginatedMessage response = new PaginatedMessage
			{
				ChannelID = command.ChannelID,
				UserID = command.UserID,
				InteractionID = command.InteractionID
			};
			response.Pages.Add(embeds);

			NetworkSystem.QueueMessage(new MessageWrapper { PaginatedMessage = response });
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
	}
}
