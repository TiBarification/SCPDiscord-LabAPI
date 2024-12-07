using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace SCPDiscord.Commands
{
  public class ListRankedCommand
  {
    [RequireGuild]
    [Command("listranked")]
    [Description("Lists online players with server ranks.")]
    public async Task OnExecute(SlashCommandContext command)
    {
      await command.DeferResponseAsync();
      Interface.MessageWrapper message = new Interface.MessageWrapper
      {
        ListRankedCommand = new Interface.ListRankedCommand
        {
          ChannelID = command.Channel.Id,
          DiscordUserID = command.User.Id,
          InteractionID = command.Interaction.Id,
          DiscordDisplayName = command.Member.DisplayName,
          DiscordUsername = command.Member.Username
        }
      };
      MessageScheduler.CacheInteraction(command);
      await NetworkSystem.SendMessage(message, command);
    }
  }
}