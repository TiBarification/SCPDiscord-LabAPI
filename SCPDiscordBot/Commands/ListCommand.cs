using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class ListCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("list", "Lists online players.")]
		public async Task OnExecute(InteractionContext command)
		{
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				ListCommand = new Interface.ListCommand
				{
					ChannelID = command.Channel.Id,
					UserID = command.User.Id,
					InteractionID = command.InteractionId,
					InteractionToken = command.Token
				}
			};
			NetworkSystem.SendMessage(message);
			await command.CreateResponseAsync("Processing...", true);
			Logger.Debug("Sending ListCommand to plugin from " + command.Member?.Username + "#" + command.Member?.Discriminator, LogID.DISCORD);
		}
	}
}
