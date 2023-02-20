using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class UnbanCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("unban", "Unbans a user from the server")]
		public async Task OnExecute(InteractionContext command, [Option("SteamIDorIP", "Steam ID or IP of the user to unban.")] string steamIDOrIP)
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				UnbanCommand = new Interface.UnbanCommand
				{
					ChannelID = command.Channel.Id,
					SteamIDOrIP = steamIDOrIP,
				}
			};
			NetworkSystem.SendMessage(message);
			Logger.Debug("Sending UnbanCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
