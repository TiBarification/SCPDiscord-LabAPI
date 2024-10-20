using System;
using System.Linq;
using CommandSystem;

namespace SCPDiscord.Commands
{
  public class SendMessageCommand : SCPDiscordCommand
  {
    public string Command { get; } = "sendmessage";
    public string[] Aliases { get; } = { "msg" };
    public string Description { get; } = "Sends a message to a Discord channel.";
    public bool SanitizeResponse { get; } = true;
    public string[] ArgumentList { get; } = { "<channel-id>", "<message>" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
      Logger.Debug(sender.LogName + " used the sendmessage command.");

      if (arguments.Count < 2)
      {
        response = "Invalid arguments.";
        return false;
      }

      if (!ulong.TryParse(arguments.At(0), out ulong channelID))
      {
        response = "Invalid channel ID.";
        return false;
      }


      SCPDiscord.plugin.SendStringByID(channelID, string.Join(" ", arguments.Skip(1)));
      response = "Message sent.";
      return true;
    }
  }
}