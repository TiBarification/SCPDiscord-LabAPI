using System;
using System.Collections.Generic;
using System.Linq;
using PluginAPI.Core;
using PluginAPI.Events;
using SCPDiscord.Interface;

namespace SCPDiscord.BotCommands
{
    public class BanCommand
    {
        public static void Execute(Interface.BanCommand command)
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
				SCPDiscord.plugin.SendEmbedWithMessageByID(embed, "messages.invalidsteamid", variables);
				return;
			}

			// Create duration timestamp.
			string humanReadableDuration = "";
			long durationSeconds = 0;
			long issuanceTime = DateTime.UtcNow.Ticks;
			DateTime endTime;
			try
			{
				endTime = ParseBanDuration(command.Duration, ref humanReadableDuration, ref durationSeconds);
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
				SCPDiscord.plugin.SendEmbedWithMessageByID(embed, "messages.invalidduration", variables);
				return;
			}

			string name = "";
			if (!SCPDiscord.plugin.GetPlayerName(command.SteamID, ref name))
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

			if (Player.TryGet(command.SteamID.EndsWith("@steam") ? command.SteamID : command.SteamID + "@steam", out Player player))
			{
				PlayerBannedEvent eventArgs = new PlayerBannedEvent(player.ReferenceHub, Server.Instance.ReferenceHub, command.Reason, durationSeconds);
				if (!EventManager.ExecuteEvent<bool>(eventArgs))
				{
					return;
				}
				BanHandler.IssueBan(new BanDetails()
				{
					OriginalName = name,
					Id = player.ReferenceHub.connectionToClient.address,
					IssuanceTime = issuanceTime,
					Expires = endTime.Ticks,
					Reason = command.Reason,
					Issuer = command.AdminTag
				}, BanHandler.BanType.IP);
				ServerConsole.Disconnect(player.ReferenceHub.gameObject, "You have been banned. Reason: " + command.Reason);
			}

			BanHandler.IssueBan(new BanDetails()
			{
				OriginalName = name,
				Id = (command.SteamID.EndsWith("@steam") ? command.SteamID : command.SteamID + "@steam"),
				IssuanceTime = issuanceTime,
				Expires = endTime.Ticks,
				Reason = command.Reason,
				Issuer = command.AdminTag
			}, BanHandler.BanType.UserId);

			BanHandler.ValidateBans();

			Dictionary<string, string> banVars = new Dictionary<string, string>
			{
				{ "name",       name                   },
				{ "steamid",    command.SteamID        },
				{ "reason",     command.Reason         },
				{ "duration",   humanReadableDuration  },
				{ "admintag",   command.AdminTag       }
			};

			embed.Colour = EmbedMessage.Types.DiscordColour.Green;
			SCPDiscord.plugin.SendEmbedWithMessageByID(embed, "messages.playerbanned", banVars);
		}

		private static DateTime ParseBanDuration(string duration, ref string humanReadableDuration, ref long durationSeconds)
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

			durationSeconds = (long)timeSpanDuration.TotalSeconds;
			return DateTime.UtcNow.Add(timeSpanDuration);
		}
    }
}