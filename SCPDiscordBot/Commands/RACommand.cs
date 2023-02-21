using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class RACommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("ra", "Runs a remote admin command.")]
		public async Task OnExecute(InteractionContext command, [Option("Command", "Remote admin command to run.")] string serverCommand = "")
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				ConsoleCommand = new Interface.ConsoleCommand
				{
					ChannelID = command.Channel.Id,
					DiscordID = command.Member?.Id ?? 0,
					Command = "/" + serverCommand
				}
			};
			NetworkSystem.SendMessage(message);
			Logger.Debug("Sending ConsoleCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
