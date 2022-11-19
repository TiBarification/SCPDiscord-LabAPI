using System.Collections.Generic;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

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

		/* TODO: [PluginEvent(ServerEventType)]
		public void OnAdminQuery(AdminQueryEvent ev)
		{
			// Triggered whenever an admin uses an admin command, both gui and commandline RA
			if (ev.Query == "REQUEST_DATA PLAYER_LIST SILENT")
			{
				return;
			}

			if (ev.Admin == null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "ipaddress",      ""                                  },
					{ "name",           "Server"                            },
					{ "playerid",       ""                                  },
					{ "steamid",        ""                                  },
					{ "class",          ""                                  },
					{ "team",           ""                                  },
					{ "handled",        ev.Handled.ToString()               },
					{ "output",         ev.Output                           },
					{ "query",          ev.Query                            },
					{ "successful",     ev.Successful.ToString()            }
				};

				plugin.SendMessage(Config.GetArray("channels.onadminquery"), "admin.onadminquery", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "ipaddress",      ev.Admin.IPAddress                  },
					{ "name",           ev.Admin.Name                       },
					{ "playerid",       ev.Admin.PlayerID.ToString()        },
					{ "steamid",        ev.Admin.GetParsedUserID()          },
					{ "class",          ev.Admin.PlayerRole.RoleID.ToString()     },
					{ "team",           ev.Admin.PlayerRole.Team.ToString()   },
					{ "handled",        ev.Handled.ToString()               },
					{ "output",         ev.Output                           },
					{ "query",          ev.Query                            },
					{ "successful",     ev.Successful.ToString()            }
				};

				plugin.SendMessage(Config.GetArray("channels.onadminquery"), "admin.onadminquery", variables);
			}
		}
		*/

		[PluginEvent(ServerEventType.PlayerBanned)]
		public void OnBan(Player player, Player issuer, string reason, long duration)
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
				plugin.SendMessage(Config.GetArray("channels.onkick"), "admin.onkick", variables);
			}
			else
			{
				plugin.SendMessage(Config.GetArray("channels.onban"), "admin.onban", variables);
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

			plugin.SendMessage(Config.GetArray("channels.onkick"), "admin.onkick", variables);
		}
	}
}