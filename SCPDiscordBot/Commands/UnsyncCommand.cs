﻿using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace SCPDiscord.Commands
{
	public class UnsyncCommand : ApplicationCommandModule
	{
		[SlashRequireGuild]
		[SlashCommand("unsync", "Unsyncs your Discord account from the SCP:SL server.")]
		public async Task OnExecute(InteractionContext command)
		{
			await command.DeferAsync();
			Interface.MessageWrapper message = new Interface.MessageWrapper
			{
				UnsyncRoleCommand = new Interface.UnsyncRoleCommand
				{
					ChannelID = command.Channel.Id,
					DiscordUserID = command.Member?.Id ?? 0,
					InteractionID = command.InteractionId,
					DiscordDisplayName = command.Member.DisplayName,
					DiscordUsername = command.Member.Username
				}
			};
			MessageScheduler.CacheInteraction(command);
			await NetworkSystem.SendMessage(message, command);
			Logger.Debug("Sending UnsyncCommand to plugin from @" + command.Member?.Username, LogID.DISCORD);
		}
	}
}
