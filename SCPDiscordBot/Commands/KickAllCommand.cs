using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class KickAllCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("kickall", "Kicks all players on the server.")]
		public async Task OnExecute(InteractionContext command, [Option("Reason", "Kick reason.")] string kickReason = "")
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				KickallCommand = new Interface.KickallCommand
				{
					ChannelID = command.Channel.Id,
					AdminTag = command.Member?.Username + "#" + command.Member?.Discriminator,
					Reason = kickReason,
					InteractionID = command.InteractionId,
					InteractionToken = command.Token
				}
			};
			await command.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
			NetworkSystem.SendMessage(message);
			Logger.Debug("Sending KickallCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
