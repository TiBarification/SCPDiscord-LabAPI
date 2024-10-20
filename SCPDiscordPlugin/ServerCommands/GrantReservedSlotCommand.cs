using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandSystem;

namespace SCPDiscord.Commands
{
  public class GrantReservedSlotCommand : SCPDiscordCommand
  {
    public string Command { get; } = "grantreservedslot";
    public string[] Aliases { get; } = { "grs" };
    public string Description { get; } = "Adds a user to the reserved slots list and reloads it.";
    public bool SanitizeResponse { get; } = true;
    public string[] ArgumentList { get; } = { "<steamid>" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
      Logger.Debug(sender.LogName + " used the grantreservedslot command.");

      if (arguments.Count < 1)
      {
        response = "Invalid arguments.";
        return false;
      }

      string steamID = arguments.At(0).Trim();
      if (!steamID.EndsWith("@steam") && long.TryParse(steamID, out _))
      {
        steamID += "@steam";
      }

      if (!Regex.IsMatch(steamID, "[0-9]+@steam"))
      {
        response = "Invalid Steam ID provided!";
        return false;
      }

      string[] reservedSlotsFileRows = File.ReadAllLines(Config.GetReservedSlotPath());
      if (reservedSlotsFileRows.Any(row => row.Trim().StartsWith(steamID)))
      {
        response = "User already has a reserved slot!";
        return false;
      }

      if (arguments.Count > 1) // Add with comment
      {
        File.AppendAllLines(Config.GetReservedSlotPath(), new[] { "# SCPDiscord: " + string.Join(" ", arguments.Skip(1)), steamID });
      }
      else // Add without comment
      {
        File.AppendAllLines(Config.GetReservedSlotPath(), new[] { steamID });
      }

      ReservedSlot.Reload();
      response = "Reserved slot added.";
      return true;
    }
  }
}