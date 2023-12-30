using System;
using System.Collections.Generic;
using CommandSystem;
using LiteNetLib;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
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

		[PluginEvent]
		public void OnPlayerBanned(PlayerBannedEvent ev)
		{
			if (!(ev.Player is Player player)) return;

			if (ev.Issuer != null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",        Utilities.SecondsToCompoundTime(ev.Duration)  },
					{ "reason",          ev.Reason                                     },
					{ "playeripaddress", player.IpAddress                              },
					{ "playername",      player.Nickname                               },
					{ "playerplayerid",  player.PlayerId.ToString()                    },
					{ "playersteamid",   player.GetParsedUserID()                      },
					{ "playerclass",     player.Role.ToString()                        },
					{ "playerteam",      player.ReferenceHub.GetTeam().ToString()      },
					{ "issueripaddress", ev.Issuer.IpAddress                           },
					{ "issuername",      ev.Issuer.Nickname                            },
					{ "issuerplayerid",  ev.Issuer.PlayerId.ToString()                 },
					{ "issuersteamid",   ev.Issuer.GetParsedUserID()                   },
					{ "issuerclass",     ev.Issuer.Role.ToString()                     },
					{ "issuerteam",      ev.Issuer.ReferenceHub.GetTeam().ToString()   }
				};

				if (ev.Duration == 0)
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
					{ "duration",        Utilities.SecondsToCompoundTime(ev.Duration)  },
					{ "reason",          ev.Reason                                     },
					{ "playeripaddress", player.IpAddress                           },
					{ "playername",      player.Nickname                            },
					{ "playerplayerid",  player.PlayerId.ToString()                 },
					{ "playersteamid",   player.GetParsedUserID()                   },
					{ "playerclass",     player.Role.ToString()                     },
					{ "playerteam",      player.ReferenceHub.GetTeam().ToString()   }
				};

				if (ev.Duration == 0)
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
		public void OnPlayerKicked(Player player, ICommandSender commandSender, string reason)
		{
			if (player == null) return;

			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player issuer = Player.Get(playerSender.ReferenceHub);
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

				plugin.SendMessage("messages.onkick.player", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "reason",                 reason                                   },
					{ "playeripaddress",        player.IpAddress                         },
					{ "playername",             player.Nickname                          },
					{ "playerplayerid",         player.PlayerId.ToString()               },
					{ "playersteamid",          player.GetParsedUserID()                 },
					{ "playerclass",            player.Role.ToString()                   },
					{ "playerteam",             player.ReferenceHub.GetTeam().ToString() }
				};

				plugin.SendMessage("messages.onkick.server", variables);
			}
		}

		[PluginEvent(ServerEventType.BanIssued)]
		public void OnBanIssued(BanDetails banDetails, BanHandler.BanType banType)
		{
			if (banType == BanHandler.BanType.IP)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",   Utilities.TicksToCompoundTime(banDetails.Expires - banDetails.IssuanceTime + 1000000) },
					{ "expirytime", new DateTime(banDetails.Expires).ToString("yyyy-MM-dd HH:mm:ss")            },
					{ "issuedtime", new DateTime(banDetails.IssuanceTime).ToString("yyyy-MM-dd HH:mm:ss")       },
					{ "reason",     banDetails.Reason        },
					{ "playerip",   banDetails.Id            },
					{ "playername", banDetails.OriginalName  },
					{ "issuername", banDetails.Issuer        },
				};
				plugin.SendMessage("messages.onbanissued.ip", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",     Utilities.TicksToCompoundTime(banDetails.Expires - banDetails.IssuanceTime + 1000000) },
					{ "expirytime",   new DateTime(banDetails.Expires).ToString("yyyy-MM-dd HH:mm:ss")            },
					{ "issuedtime",   new DateTime(banDetails.IssuanceTime).ToString("yyyy-MM-dd HH:mm:ss")       },
					{ "reason",       banDetails.Reason        },
					{ "playeruserid", banDetails.Id            },
					{ "playername",   banDetails.OriginalName  },
					{ "issuername",   banDetails.Issuer        },
				};
				plugin.SendMessage("messages.onbanissued.userid", variables);
			}
		}

		[PluginEvent(ServerEventType.BanUpdated)]
		public void OnBanUpdated(BanDetails banDetails, BanHandler.BanType banType)
		{
			if (banType == BanHandler.BanType.IP)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",   Utilities.TicksToCompoundTime(banDetails.Expires - banDetails.IssuanceTime + 1000000) },
					{ "expirytime", new DateTime(banDetails.Expires).ToString("yyyy-MM-dd HH:mm:ss")            },
					{ "issuedtime", new DateTime(banDetails.IssuanceTime).ToString("yyyy-MM-dd HH:mm:ss")       },
					{ "reason",     banDetails.Reason        },
					{ "playerip",   banDetails.Id            },
					{ "playername", banDetails.OriginalName  },
					{ "issuername", banDetails.Issuer        },
				};
				plugin.SendMessage("messages.onbanupdated.ip", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",     Utilities.TicksToCompoundTime(banDetails.Expires - banDetails.IssuanceTime + 1000000) },
					{ "expirytime",   new DateTime(banDetails.Expires).ToString("yyyy-MM-dd HH:mm:ss")            },
					{ "issuedtime",   new DateTime(banDetails.IssuanceTime).ToString("yyyy-MM-dd HH:mm:ss")       },
					{ "reason",       banDetails.Reason        },
					{ "playeruserid", banDetails.Id            },
					{ "playername",   banDetails.OriginalName  },
					{ "issuername",   banDetails.Issuer        },
				};
				plugin.SendMessage("messages.onbanupdated.userid", variables);
			}
		}

		[PluginEvent(ServerEventType.BanRevoked)]
		public void OnBanRevoked(string id, BanHandler.BanType banType)
		{
			if (banType == BanHandler.BanType.IP)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "ip", id },
				};
				plugin.SendMessage("messages.onbanrevoked.ip", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "userid", id },
				};
				plugin.SendMessage("messages.onbanrevoked.userid", variables);
			}
		}

		[PluginEvent]
		public void OnPlayerMuted(PlayerMutedEvent ev)
		{
			if (ev.Player == null) return;

			if (ev.Issuer != null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "playeripaddress",        ev.Player.IpAddress                         },
					{ "playername",             ev.Player.Nickname                          },
					{ "playerplayerid",         ev.Player.PlayerId.ToString()               },
					{ "playersteamid",          ev.Player.GetParsedUserID()                 },
					{ "playerclass",            ev.Player.Role.ToString()                   },
					{ "playerteam",             ev.Player.ReferenceHub.GetTeam().ToString() },
					{ "issueripaddress",        ev.Issuer.IpAddress                         },
					{ "issuername",             ev.Issuer.Nickname                          },
					{ "issuerplayerid",         ev.Issuer.PlayerId.ToString()               },
					{ "issuersteamid",          ev.Issuer.GetParsedUserID()                 },
					{ "issuerclass",            ev.Issuer.Role.ToString()                   },
					{ "issuerteam",             ev.Issuer.ReferenceHub.GetTeam().ToString() }
				};

				if (ev.IsIntercom)
				{
					plugin.SendMessage("messages.onplayermuted.player.intercom", variables);
				}
				else
				{
					plugin.SendMessage("messages.onplayermuted.player.standard", variables);
				}
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "playeripaddress",        ev.Player.IpAddress                         },
					{ "playername",             ev.Player.Nickname                          },
					{ "playerplayerid",         ev.Player.PlayerId.ToString()               },
					{ "playersteamid",          ev.Player.GetParsedUserID()                 },
					{ "playerclass",            ev.Player.Role.ToString()                   },
					{ "playerteam",             ev.Player.ReferenceHub.GetTeam().ToString() }
				};

				if (ev.IsIntercom)
				{
					plugin.SendMessage("messages.onplayermuted.server.intercom", variables);
				}
				else
				{
					plugin.SendMessage("messages.onplayermuted.server.standard", variables);
				}
			}
		}

		[PluginEvent]
		public void OnPlayerUnmuted(PlayerUnmutedEvent ev)
		{
			if (ev.Player == null) return;

			if (ev.Issuer != null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "playeripaddress", ev.Player.IpAddress                    },
					{ "playername", ev.Player.Nickname                          },
					{ "playerplayerid", ev.Player.PlayerId.ToString()           },
					{ "playersteamid", ev.Player.GetParsedUserID()              },
					{ "playerclass", ev.Player.Role.ToString()                  },
					{ "playerteam", ev.Player.ReferenceHub.GetTeam().ToString() },
					{ "issueripaddress", ev.Issuer.IpAddress                    },
					{ "issuername", ev.Issuer.Nickname                          },
					{ "issuerplayerid", ev.Issuer.PlayerId.ToString()           },
					{ "issuersteamid", ev.Issuer.GetParsedUserID()              },
					{ "issuerclass", ev.Issuer.Role.ToString()                  },
					{ "issuerteam", ev.Issuer.ReferenceHub.GetTeam().ToString() }
				};

				if (ev.IsIntercom)
				{
					plugin.SendMessage("messages.onplayerunmuted.player.intercom", variables);
				}
				else
				{
					plugin.SendMessage("messages.onplayerunmuted.player.standard", variables);
				}
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "playeripaddress", ev.Player.IpAddress                    },
					{ "playername", ev.Player.Nickname                          },
					{ "playerplayerid", ev.Player.PlayerId.ToString()           },
					{ "playersteamid", ev.Player.GetParsedUserID()              },
					{ "playerclass", ev.Player.Role.ToString()                  },
					{ "playerteam", ev.Player.ReferenceHub.GetTeam().ToString() }
				};

				if (ev.IsIntercom)
				{
					plugin.SendMessage("messages.onplayerunmuted.server.intercom", variables);
				}
				else
				{
					plugin.SendMessage("messages.onplayerunmuted.server.standard", variables);
				}
			}
		}

		[PluginEvent(ServerEventType.RemoteAdminCommandExecuted)]
		public void OnRemoteAdminCommand(ICommandSender commandSender, string command, string[] args, bool result, string response)
		{
			if (commandSender is PlayerCommandSender playerSender && Player.Get(playerSender.ReferenceHub) != null)
			{
				Player player = Player.Get(playerSender.ReferenceHub);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "command",       (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",       (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",       (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",       (command + " " + string.Join(" ", args)).Trim() },
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
					{ "command",       (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",       (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",   (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",   (command + " " + string.Join(" ", args)).Trim()   }
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
					{ "command",   (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command",   (command + " " + string.Join(" ", args)).Trim()   }
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
					{ "command",   (command + " " + string.Join(" ", args)).Trim()   },
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
					{ "command", (command + " " + string.Join(" ", args)).Trim()   }
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

		[PluginEvent]
		public void OnConnect(PlayerPreauthEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress", ev.IpAddress                    },
				{ "steamid",   ev.UserId.Replace("@steam", "") },
				{ "jointype",  ev.CentralFlags.ToString()      },
				{ "region",    ev.Region                       }
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
			}
			plugin.roundStarted = false;
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

		[PluginEvent]
		public void OnPlayerCheaterReport(PlayerCheaterReportEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "playeripaddress", ev.Player.IpAddress                         },
				{ "playername",      ev.Player.Nickname                          },
				{ "playerplayerid",  ev.Player.PlayerId.ToString()               },
				{ "playersteamid",   ev.Player.GetParsedUserID()                 },
				{ "playerclass",     ev.Player.Role.ToString()                   },
				{ "playerteam",      ev.Player.ReferenceHub.GetTeam().ToString() },
				{ "targetipaddress", ev.Target.IpAddress                         },
				{ "targetname",      ev.Target.Nickname                          },
				{ "targetplayerid",  ev.Target.PlayerId.ToString()               },
				{ "targetsteamid",   ev.Target.GetParsedUserID()                 },
				{ "targetclass",     ev.Target.Role.ToString()                   },
				{ "targetteam",      ev.Target.ReferenceHub.GetTeam().ToString() },
				{ "reason",          ev.Reason                                   }
			};
			plugin.SendMessage("messages.onplayercheaterreport", variables);
		}

		[PluginEvent]
		public void OnPlayerReport(PlayerReportEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "playeripaddress", ev.Player.IpAddress                         },
				{ "playername",      ev.Player.Nickname                          },
				{ "playerplayerid",  ev.Player.PlayerId.ToString()               },
				{ "playersteamid",   ev.Player.GetParsedUserID()                 },
				{ "playerclass",     ev.Player.Role.ToString()                   },
				{ "playerteam",      ev.Player.ReferenceHub.GetTeam().ToString() },
				{ "targetipaddress", ev.Target.IpAddress                         },
				{ "targetname",      ev.Target.Nickname                          },
				{ "targetplayerid",  ev.Target.PlayerId.ToString()               },
				{ "targetsteamid",   ev.Target.GetParsedUserID()                 },
				{ "targetclass",     ev.Target.Role.ToString()                   },
				{ "targetteam",      ev.Target.ReferenceHub.GetTeam().ToString() },
				{ "reason",          ev.Reason                                   }
			};
			plugin.SendMessage("messages.onplayerreport", variables);
		}
	}
}