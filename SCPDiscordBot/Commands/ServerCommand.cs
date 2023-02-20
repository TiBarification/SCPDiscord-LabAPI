using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class ServerCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("ban", "Bans a user from the server")]
		public async Task OnExecute(InteractionContext command, [Option("Command", "Server console command to run, use / prefix for RA and . for client commands, no prefix for server commands.")]  string serverCommand = "")
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				ConsoleCommand = new Interface.ConsoleCommand
				{
					ChannelID = command.Channel.Id,
					DiscordID = command.Member?.Id ?? 0,
					Command = serverCommand
				}
			};
			NetworkSystem.SendMessage(message);
			Logger.Debug("Sending ConsoleCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
