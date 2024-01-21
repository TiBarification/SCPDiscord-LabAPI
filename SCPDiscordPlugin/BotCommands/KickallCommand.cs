using System.Collections.Generic;
using PluginAPI.Core;
using SCPDiscord.Interface;

namespace SCPDiscord.BotCommands
{
    public static class KickallCommand
    {
        public static void Execute(Interface.KickallCommand command)
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
            SCPDiscord.plugin.SendEmbedWithMessageByID(embed, "messages.kickall", variables);
        }
    }
}