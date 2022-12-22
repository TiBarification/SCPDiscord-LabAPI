using System.Collections.Generic;
using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RemoteAdmin;

namespace SCPDiscord.EventListeners
{
	//Comments here are my own as there were none in the Smod2 api
	internal class AdminEventListener
	{
		private readonly SCPDiscord plugin;

		public AdminEventListener(SCPDiscord plugin)
		{
			this.plugin = plugin;
		}

		[PluginEvent(ServerEventType.PlayerBanned)]
		public void OnBan(Player player, ICommandSender commandSender, string reason, long duration)
		{
			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player issuer = Player.Get(playerSender.ReferenceHub);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",               Utilities.SecondsToCompoundTime(duration)  },
					{ "reason",                 reason                                     },
					{ "playeripaddress",        player.IpAddress                           },
					{ "playername",             player.Nickname                            },
					{ "playerplayerid",         player.PlayerId.ToString()                 },
					{ "playersteamid",          player.GetParsedUserID()                   },
					{ "playerclass",            player.Role.ToString()                     },
					{ "playerteam",             player.ReferenceHub.GetTeam().ToString()   },
					{ "issueripaddress",        issuer.IpAddress                           },
					{ "issuername",             issuer.Nickname                            },
					{ "issuerplayerid",         issuer.PlayerId.ToString()                 },
					{ "issuersteamid",          issuer.GetParsedUserID()                   },
					{ "issuerclass",            issuer.Role.ToString()                     },
					{ "issuerteam",             issuer.ReferenceHub.GetTeam().ToString()   }
				};

				if (duration == 0)
				{
					plugin.SendMessage("messages.onkick.player", variables);
				}
				else
				{
					plugin.SendMessage("messages.onban.player", variables);
				}
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",               Utilities.SecondsToCompoundTime(duration)  },
					{ "reason",                 reason                                     },
					{ "playeripaddress",        player.IpAddress                           },
					{ "playername",             player.Nickname                            },
					{ "playerplayerid",         player.PlayerId.ToString()                 },
					{ "playersteamid",          player.GetParsedUserID()                   },
					{ "playerclass",            player.Role.ToString()                     },
					{ "playerteam",             player.ReferenceHub.GetTeam().ToString()   }
				};

				if (duration == 0)
				{
					plugin.SendMessage("messages.onkick.server", variables);
				}
				else
				{
					plugin.SendMessage("messages.onban.server", variables);
				}
			}
		}

		[PluginEvent(ServerEventType.PlayerKicked)]
		public void OnKick(Player player, Player issuer, string reason)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "reason",                 reason                                   },
				{ "playeripaddress",        player.IpAddress                         },
				{ "playername",             player.Nickname                          },
				{ "playerplayerid",         player.PlayerId.ToString()               },
				{ "playersteamid",          player.GetParsedUserID()                 },
				{ "playerclass",            player.Role.ToString()                   },
				{ "playerteam",             player.ReferenceHub.GetTeam().ToString() },
				{ "issueripaddress",        issuer.IpAddress                         },
				{ "issuername",             issuer.Nickname                          },
				{ "issuerplayerid",         issuer.PlayerId.ToString()               },
				{ "issuersteamid",          issuer.GetParsedUserID()                 },
				{ "issuerclass",            issuer.Role.ToString()                   },
				{ "issuerteam",             issuer.ReferenceHub.GetTeam().ToString() }
			};

			plugin.SendMessage("messages.onkick", variables);
		}
	}
}