using System.ComponentModel;
using System.Threading.Tasks;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace SCPDiscord.Commands
{
  public class HelpCommand
  {
    [RequireGuild]
    [Command("help")]
    [Description("Shows basic bot information.")]
    public async Task OnExecute(SlashCommandContext command)
    {
      DiscordEmbed botInfo = new DiscordEmbedBuilder()
        .WithAuthor("KarlofDuty/SCPDiscord @ GitHub", "https://github.com/KarlofDuty/SCPDiscord", "https://karlofduty.com/img/tardisIcon.jpg")
        .WithTitle("Bot information")
        .WithColor(DiscordColor.Cyan)
        .AddField("Version:", SCPDiscordBot.GetVersion(), true)
        .AddField("Connected:", NetworkSystem.IsConnected() ? "Yes" : "No", true)
        .AddField("\u200b", "\u200b", true)
        .AddField("Report bugs:", "[Github Issues](https://github.com/KarlofDuty/SCPDiscord/issues)", true)
        .AddField("Commands:", "[Github Repository](https://github.com/KarlOfDuty/SCPDiscord/blob/master/docs/Usage.md)", true)
        .AddField("Donate:", "[Github Sponsors](https://github.com/sponsors/KarlOfDuty)", true);
      await command.RespondAsync(botInfo);
    }
  }
}