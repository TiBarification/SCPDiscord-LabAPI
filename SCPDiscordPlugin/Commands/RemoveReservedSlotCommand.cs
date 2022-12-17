using System;
using CommandSystem;

namespace SCPDiscord.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof (GameConsoleCommandHandler))]
	public class RemoveReservedSlotCommand : ICommand
	{
		public string Command => "scpdiscord_removereservedslot";
		public string[] Aliases => new string[] { "scpd_removereservedslot", "scpd_rrs" };
		public string Description => "Removes a reserved slot from a player.";
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "This command doesn't work in this version.";
			return false;
			/*
			if (sender is Player player)
			{
				if (!player.HasPermission("scpdiscord.removereservedslot"))
				{
					return new[] { "You don't have permission to use that command." };
				}
			}

			if (args.Length <= 0)
			{
				return new[] { "Invalid arguments." };
			}

			if (ReservedSlot.GetSlots().All(slot => slot.SteamID != args[0].Trim()))
			{
				return new[] { "This user does not have a reserved slot." };
			}

			ReservedSlot.GetSlots().First(slot => slot.SteamID == args[0])?.RemoveSlotFromFile();

			return new[] { "Reserved slot removed." };
			*/
		}
	}
}