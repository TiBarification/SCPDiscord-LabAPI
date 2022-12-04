using PluginAPI.Core;

namespace SCPDiscord
{
	public static class APIExtensions
	{
		public static string GetParsedUserID(this Player player)
		{
			if (!string.IsNullOrWhiteSpace(player.UserId))
			{
				int charLocation = player.UserId.LastIndexOf('@');

				if (charLocation > 0)
				{
					return player.UserId.Substring(0, charLocation);
				}
			}
			return null;
		}

		public static void SetRank(this Player player, string color = null, string text = null, string group = null)
		{
			ServerRoles roleComponent = player.ReferenceHub.serverRoles;

			if (roleComponent != null)
			{
				// TODO: Fix vanilla ranks
				//if (group != null)
					//roleComponent.SetGroup(ServerStatic.PermissionsHandler.GetGroup(group), false);
				if (color != null)
					roleComponent.SetColor(color);
				if (text != null)
					roleComponent.SetText(text);
			}
		}
	}
}