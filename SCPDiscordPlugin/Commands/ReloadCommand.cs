using System;
using CommandSystem;

namespace SCPDiscord.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof (GameConsoleCommandHandler))]
	public class ReloadCommand : ICommand
	{
		public string Command => "scpdiscord_reload";
		public string[] Aliases => new string[] { "scpd_reload" };
		public string Description => "Reloads all plugin configs and data files and then reconnects to the bot.";
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			/*
			if (sender is Player player)
			{
				if (!player.HasPermission("scpdiscord.reload"))
				{
					return new[] { "You don't have permission to use that command." };
				}
			}
			*/

			SCPDiscord.plugin.Info("Reloading plugin...");
			SCPDiscord.plugin.LoadConfig();
			Language.Reload();
			SCPDiscord.plugin.roleSync.Reload();
			if (NetworkSystem.IsConnected())
			{
				NetworkSystem.Disconnect();
			}

			response = "Reload complete.";
			return true;
		}
	}
}