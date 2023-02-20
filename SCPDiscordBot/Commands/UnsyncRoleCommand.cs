using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class UnsyncRoleCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("unsyncrole", "Unsyncs your Discord account from the SCP:SL server.")]
		public async Task OnExecute(InteractionContext command)
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				UnsyncRoleCommand = new Interface.UnsyncRoleCommand
				{
					ChannelID = command.Channel.Id,
					DiscordID = command.Member?.Id ?? 0,
					DiscordTag = command.Member?.Username
				}
			};
			NetworkSystem.SendMessage(message);
			Logger.Debug("Sending UnsyncRoleCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
