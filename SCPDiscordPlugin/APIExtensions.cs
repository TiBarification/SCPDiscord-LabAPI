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
	}
}