using System;
using CommandSystem;

namespace SCPDiscord.Commands
{
	public class UnsyncCommand : SCPDiscordCommand
	{
		public string Command { get; } = "unsync";
		public string[] Aliases { get; } = { };
		public string Description { get; } = "Removes a user from having their discord role synced to the server.";
		public string[] ArgumentList { get; } = { "<discordid>" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			Logger.Debug(sender.LogName + "used the unsync command.");

			if (arguments.Count > 0 && ulong.TryParse(arguments.At(0), out ulong discordID))
			{
				response = RoleSync.RemovePlayerLocally(discordID);
				return true;
			}

			response = "Invalid arguments.";
			return false;
		}
	}
}