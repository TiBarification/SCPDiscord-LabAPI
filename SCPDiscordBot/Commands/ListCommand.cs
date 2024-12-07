using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;

namespace SCPDiscord.Commands
{
  public class ListCommand
  {
    [RequireGuild]
    [Command("list")]
    [Description("Lists online players.")]
    public async Task OnExecute(SlashCommandContext command)
    {
      await command.DeferResponseAsync();
      Interface.MessageWrapper message = new Interface.MessageWrapper
      {
        ListCommand = new Interface.ListCommand
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