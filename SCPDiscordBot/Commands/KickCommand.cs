using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class KickCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("kick", "Kicks a player from the server.")]
		public async Task OnExecute(InteractionContext command, [Option("SteamID", "Steam ID of the user to kick.")] string steamID, [Option("Reason", "Reason for the kick.")] string reason = "")
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				KickCommand = new Interface.KickCommand
				{
					ChannelID = command.Channel.Id,
					SteamID = steamID,
					AdminTag = command.Member?.Username + "#" + command.Member?.Discriminator,
					Reason = reason
				}
			};
			NetworkSystem.SendMessage(message);
			Logger.Debug("Sending KickCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
