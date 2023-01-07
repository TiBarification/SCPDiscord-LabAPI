using System;
using System.Collections.Generic;
using CommandSystem;
using LiteNetLib;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RemoteAdmin;

namespace SCPDiscord.EventListeners
{
	internal class ServerEventListener
	{
		private readonly SCPDiscord plugin;

		public ServerEventListener(SCPDiscord plugin)
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

		[PluginEvent(ServerEventType.RemoteAdminCommandExecuted)]
		public void OnRemoteAdminCommand(ICommandSender commandSender, string command, string[] args, bool result, string response)
		{
			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player player = Player.Get(playerSender.ReferenceHub);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       command + " " + string.Join(" ", args)   },
					{ "result",        result.ToString()                        },
					{ "returnmessage", response                                 },
					{ "ipaddress",     player.IpAddress                         },
					{ "name",          player.Nickname                          },
					{ "playerid",      player.PlayerId.ToString()               },
					{ "steamid",       player.GetParsedUserID()                 },
					{ "class",         player.Role.ToString()                   },
					{ "team",          player.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.onexecutedcommand.remoteadmin.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       command + " " + string.Join(" ", args)   },
					{ "result",        result.ToString()                        },
					{ "returnmessage", response                                 }
				};
				plugin.SendMessage("messages.onexecutedcommand.remoteadmin.server", variables);
			}
		}

		[PluginEvent(ServerEventType.PlayerGameConsoleCommandExecuted)]
		public void OnGameConsoleCommand(Player player, string command, string[] args, bool result, string response)
		{
			if (player != null && player.PlayerId != Server.Instance.PlayerId)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       command + " " + string.Join(" ", args)   },
					//{ "result",        result.ToString()                        },
					{ "returnmessage", response                                 },
					{ "ipaddress",     player.IpAddress                         },
					{ "name",          player.Nickname                          },
					{ "playerid",      player.PlayerId.ToString()               },
					{ "steamid",       player.GetParsedUserID()                 },
					{ "class",         player.Role.ToString()                   },
					{ "team",          player.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.onexecutedcommand.game.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       command + " " + string.Join(" ", args) },
					//{ "result",        result.ToString()                      },
					{ "returnmessage", response                               }
				};
				plugin.SendMessage("messages.onexecutedcommand.game.server", variables);
			}
		}

		[PluginEvent(ServerEventType.ConsoleCommandExecuted)]
		public void OnConsoleCommand(ICommandSender commandSender, string command, string[] args, bool result, string response)
		{
			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player player = Player.Get(playerSender.ReferenceHub);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       command + " " + string.Join(" ", args)   },
					{ "result",        result.ToString()                        },
					{ "ipaddress",     player.IpAddress                         },
					{ "name",          player.Nickname                          },
					{ "playerid",      player.PlayerId.ToString()               },
					{ "steamid",       player.GetParsedUserID()                 },
					{ "class",         player.Role.ToString()                   },
					{ "team",          player.ReferenceHub.GetTeam().ToString() },
					{ "result",        result.ToString()                        },
					{ "returnmessage", response                                 }
				};
				plugin.SendMessage("messages.onexecutedcommand.console.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       command + " " + string.Join(" ", args)   },
					{ "result",        result.ToString()                        },
					{ "returnmessage", response                                 }
				};
				plugin.SendMessage("messages.onexecutedcommand.console.server", variables);
			}
		}

		[PluginEvent(ServerEventType.RemoteAdminCommand)]
		public void OnRemoteAdminCommand(ICommandSender commandSender, string command, string[] args)
		{
			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player player = Player.Get(playerSender.ReferenceHub);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",   command + " " + string.Join(" ", args)   },
					{ "ipaddress", player.IpAddress                         },
					{ "name",      player.Nickname                          },
					{ "playerid",  player.PlayerId.ToString()               },
					{ "steamid",   player.GetParsedUserID()                 },
					{ "class",     player.Role.ToString()                   },
					{ "team",      player.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.oncallcommand.remoteadmin.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",   command + " " + string.Join(" ", args)   }
				};
				plugin.SendMessage("messages.oncallcommand.remoteadmin.server", variables);
			}
		}

		[PluginEvent(ServerEventType.PlayerGameConsoleCommand)]
		public void OnGameConsoleCommand(Player player, string command, string[] args)
		{
			if (player != null && player.PlayerId != Server.Instance.PlayerId)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",   command + " " + string.Join(" ", args)   },
					{ "ipaddress", player.IpAddress                         },
					{ "name",      player.Nickname                          },
					{ "playerid",  player.PlayerId.ToString()               },
					{ "steamid",   player.GetParsedUserID()                 },
					{ "class",     player.Role.ToString()                   },
					{ "team",      player.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.oncallcommand.game.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",   command + " " + string.Join(" ", args)   }
				};
				plugin.SendMessage("messages.oncallcommand.game.server", variables);
			}
		}

		[PluginEvent(ServerEventType.ConsoleCommand)]
		public void OnConsoleCommand(ICommandSender commandSender, string command, string[] args)
		{
			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player player = Player.Get(playerSender.ReferenceHub);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",   command + " " + string.Join(" ", args)   },
					{ "ipaddress", player.IpAddress                         },
					{ "name",      player.Nickname                          },
					{ "playerid",  player.PlayerId.ToString()               },
					{ "steamid",   player.GetParsedUserID()                 },
					{ "class",     player.Role.ToString()                   },
					{ "team",      player.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.oncallcommand.console.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command", command + " " + string.Join(" ", args)   }
				};
				plugin.SendMessage("messages.oncallcommand.console.server", variables);
			}
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void OnRoundStart()
		{
			plugin.SendMessage("messages.onroundstart");
			plugin.roundStarted = true;
		}

		[PluginEvent(ServerEventType.PlayerPreauth)]
		public void OnConnect(string userID, string ipAddress, long expiration, CentralAuthPreauthFlags flags, string region, byte[] signature, ConnectionRequest connectionRequest, int readerStartPosition)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress", ipAddress       },
				{ "steamid", userID            },
				{ "jointype", flags.ToString() },
				{ "region", region             }
			};
			plugin.SendMessage("messages.onconnect", variables);
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void OnRoundEnd(RoundSummary.LeadingTeam leadingTeam)
		{
			if (plugin.roundStarted && new TimeSpan(DateTime.Now.Ticks - Statistics.CurrentRound.StartTimestamp.Ticks).TotalSeconds > 60)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",          (new TimeSpan(DateTime.Now.Ticks - Statistics.CurrentRound.StartTimestamp.Ticks).TotalSeconds / 60).ToString("0") },
					{ "leadingteam",        leadingTeam.ToString()                               },
					{ "dclassalive",        Statistics.CurrentRound.ClassDAlive.ToString()       },
					{ "dclassdead",         Statistics.CurrentRound.ClassDDead.ToString()        },
					{ "dclassescaped",      Statistics.CurrentRound.ClassDEscaped.ToString()     },
					{ "dclassstart",        Statistics.CurrentRound.ClassDStart.ToString()       },
					{ "mtfalive",           Statistics.CurrentRound.MtfAndGuardsAlive.ToString() },
					{ "mtfdead",            Statistics.CurrentRound.MtfAndGuardsDead.ToString()  },
					{ "mtfstart",           Statistics.CurrentRound.MtfAndGuardsStart.ToString() },
					{ "scientistsalive",    Statistics.CurrentRound.ScientistsAlive.ToString()   },
					{ "scientistsdead",     Statistics.CurrentRound.ScientistsDead.ToString()    },
					{ "scientistsescaped",  Statistics.CurrentRound.ScientistsEscaped.ToString() },
					{ "scientistsstart",    Statistics.CurrentRound.ScientistsStart.ToString()   },
					{ "scpalive",           Statistics.CurrentRound.ScpsAlive.ToString()         },
					{ "scpdead",            Statistics.CurrentRound.ScpsDead.ToString()          },
					{ "scpkills",           Statistics.CurrentRound.TotalScpKills.ToString()     },
					{ "scpstart",           Statistics.CurrentRound.ScpsStart.ToString()         },
					{ "warheaddetonated",   Statistics.CurrentRound.WarheadDetonated.ToString()  },
					{ "warheadkills",       Statistics.CurrentRound.WarheadKills.ToString()      },
					{ "zombiesalive",       Statistics.CurrentRound.ZombiesAlive.ToString()      },
					{ "zombieschanged",     Statistics.CurrentRound.ZombiesChanged.ToString()    }
				};
				plugin.SendMessage("messages.onroundend", variables);
				plugin.roundStarted = false;
			}
		}

		[PluginEvent(ServerEventType.WaitingForPlayers)]
		public void OnWaitingForPlayers()
		{
			plugin.SendMessage("messages.onwaitingforplayers");
		}

		[PluginEvent(ServerEventType.RoundRestart)]
		public void OnRoundRestart()
		{
			plugin.SendMessage("messages.onroundrestart");
		}

		/*
		[PluginEvent(ServerEventType.)]
		public void OnSetServerName(SetServerNameEvent ev)
		{
			ev.ServerName = (ConfigManager.Manager.Config.GetBoolValue("discord_metrics", true)) ? ev.ServerName += "<color=#ffffff00><size=1>SCPD:" + this.plugin.Details.version + "</size></color>" : ev.ServerName;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "servername", ev.ServerName }
			};
			this.plugin.SendMessage(Config.GetArray("messages.onsetservername"), "messages.onsetservername", variables);
		}
		*/
	}
}