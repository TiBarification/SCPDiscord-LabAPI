using System;
using CommandSystem;

namespace SCPDiscord.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GrantReservedSlotCommand : ICommand
	{
		public string Command => "scpdiscord_grantreservedslot";
		public string[] Aliases => new string[] { "scpd_grantreservedslot", "scpd_grs" };
		public string Description => "Gives a user a reserved slot.";
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "This command doesn't work in this version.";
			return false;
			/*
			if (sender is Player admin)
			{
				if (!admin.HasPermission("scpdiscord.grantreservedslot"))
				{
					return new[] { "You don't have permission to use that command." };
				}
			}

			if (args.Length <= 0)
			{
				return new[] { "Invalid arguments." };
			}

			if (ReservedSlot.GetSlots().Any(slot => slot.SteamID == args[0].Trim()))
			{
				return new[] { "This user already has a reserved slot." };
			}

			try
			{
				Player player = SCPDiscord.plugin.Server.GetPlayers(args[0]).First();
				new ReservedSlot(player.IpAddress, player.UserId, player.Name + ", added via SCPDiscord " + DateTime.Now).AppendToFile();
			}
			catch (InvalidOperationException)
			{
				new ReservedSlot("", args[0], "Offline player added via SCPDiscord " + DateTime.Now).AppendToFile();
			}
			return new[] { "Reserved slot added." };
			*/
		}
	}
}