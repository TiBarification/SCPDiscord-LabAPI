using System;
using System.Collections.Generic;
using PluginAPI.Core;
using SCPDiscord.Interface;
namespace SCPDiscord.BotCommands
{
    public static class ListCommand
    {
        public static void Execute(Interface.ListCommand command)
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
				SCPDiscord.plugin.SendEmbedByID(embed);
				return;
			}

			List<string> listItems = new List<string>();
			foreach (Player player in Player.GetPlayers())
			{
				Dictionary<string, string> variables = new Dictionary<string, string> {};
				variables.AddPlayerVariables(player, "player");
				string row = Language.GetProcessedMessage("messages.listrow.default", variables);

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
    }
}