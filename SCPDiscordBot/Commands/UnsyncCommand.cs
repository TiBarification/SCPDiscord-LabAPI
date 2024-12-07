using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace SCPDiscord.Commands;

public class UnsyncCommand
{
  [RequireGuild]
  [Command("unsync")]
  [Description("Unsyncs your Discord account from the SCP:SL server.")]
  public async Task OnExecute(SlashCommandContext command)
  {
    await command.DeferResponseAsync();
    Interface.MessageWrapper message = new Interface.MessageWrapper
    {
      UnsyncRoleCommand = new Interface.UnsyncRoleCommand
      {
        ChannelID = command.Channel.Id,
        DiscordUserID = command.Member?.Id ?? 0,
        InteractionID = command.Interaction.Id,
        DiscordDisplayName = command.Member.DisplayName,
        DiscordUsername = command.Member.Username
      }
    };
    MessageScheduler.CacheInteraction(command);
    await NetworkSystem.SendMessage(message, command);
  }
}