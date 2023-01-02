using System.Collections.Generic;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using MapGeneration.Distributors;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Scp914;

namespace SCPDiscord.EventListeners
{
	internal class EnvironmentEventListener
	{
		private readonly SCPDiscord plugin;

		public EnvironmentEventListener(SCPDiscord pl)
		{
			plugin = pl;
		}

		[PluginEvent(ServerEventType.PlayerInteractDoor)]
		public void OnDoorAccess(Player player, DoorVariant door, bool canOpen)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "doorname",       door.name                                },
				{ "permission",     door.RequiredPermissions.RequiredPermissions.ToString()    },
				//{ "locked",         ev.Door.IsLocked.ToString()              },
				//{ "open",           door.TargetState.ToString()              },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage(canOpen ? "messages.ondooraccess.allowed" : "messages.ondooraccess.denied", variables);
		}

		[PluginEvent(ServerEventType.PlayerExitPocketDimension)]
		public void OnPocketDimensionExit(Player player, bool isSuccessful)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "successful",     isSuccessful.ToString()                  },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onpocketdimensionexit", variables);
		}

		[PluginEvent(ServerEventType.Scp106TeleportPlayer)]
		public void OnPocketDimensionEnter(Player scp106, Player player)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "attackeripaddress",  scp106.IpAddress                         },
				{ "attackername",       scp106.Nickname                          },
				{ "attackerplayerid",   scp106.PlayerId.ToString()               },
				{ "attackersteamid",    scp106.GetParsedUserID()                 },
				{ "attackerclass",      scp106.Role.ToString()                   },
				{ "attackerteam",       scp106.ReferenceHub.GetTeam().ToString() },
				{ "playeripaddress",    player.IpAddress                         },
				{ "playername",         player.Nickname                          },
				{ "playerplayerid",     player.PlayerId.ToString()               },
				{ "playersteamid",      player.GetParsedUserID()                 },
				{ "playerclass",        player.Role.ToString()                   },
				{ "playerteam",         player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onpocketdimensionenter", variables);
		}

		[PluginEvent(ServerEventType.Scp914Activate)]
		public void OnSCP914Activate(Player player, Scp914KnobSetting setting)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "knobsetting",    setting.ToString()   },
				{ "ipaddress",   player.IpAddress                           },
				{ "name",        player.Nickname                            },
				{ "playerid",    player.PlayerId.ToString()                 },
				{ "steamid",     player.GetParsedUserID()                   },
				{ "class",       player.Role.ToString()                     },
				{ "team",        player.ReferenceHub.GetTeam().ToString()   }
			};
			plugin.SendMessage("messages.onscp914activate", variables);
		}

		[PluginEvent(ServerEventType.Scp914KnobChange)]
		public void OnSCP914ChangeKnob(Player player, Scp914KnobSetting newSetting, Scp914KnobSetting oldSetting)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "newsetting",     newSetting.ToString()                    },
				{ "oldsetting",     oldSetting.ToString()                    },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onscp914changeknob", variables);
		}

		[PluginEvent(ServerEventType.PlayerInteractElevator)]
		public void OnElevatorUse(Player player, ElevatorChamber elevator)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "elevatorname",   elevator.name                            },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onelevatoruse", variables);
		}

		[PluginEvent(ServerEventType.WarheadStart)]
		public void OnStartCountdown(bool isAutomatic, Player player, bool isResumed)
		{
			if (player == null || player.PlayerId == Server.Instance.PlayerId)
			{
				Dictionary<string, string> vars = new Dictionary<string, string>
				{
					{ "isAutomatic",    isAutomatic.ToString()                 },
					{ "timeleft",    Warhead.DetonationTime.ToString("0") },
				};
				plugin.SendMessage("messages.onstartcountdown.noplayer", vars);
				return;
			}

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "isAutomatic", isAutomatic.ToString()                     },
				{ "timeleft",    Warhead.DetonationTime.ToString("0") },
				{ "ipaddress",   player.IpAddress                           },
				{ "name",        player.Nickname                            },
				{ "playerid",    player.PlayerId.ToString()                 },
				{ "steamid",     player.GetParsedUserID()                   },
				{ "class",       player.Role.ToString()                     },
				{ "team",        player.ReferenceHub.GetTeam().ToString()   }
			};

			plugin.SendMessage(isResumed ? "messages.onstartcountdown.resumed" : "messages.onstartcountdown.initiated", variables);
		}

		[PluginEvent(ServerEventType.WarheadStop)]
		public void OnStopCountdown(Player player)
		{
			if (player == null || player.PlayerId == Server.Instance.PlayerId)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "timeleft",    Warhead.DetonationTime.ToString("0.##") },
				};
				plugin.SendMessage("messages.onstopcountdown.noplayer", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "timeleft",    Warhead.DetonationTime.ToString("0.##") },
					{ "ipaddress",   player.IpAddress                         },
					{ "name",        player.Nickname                          },
					{ "playerid",    player.PlayerId.ToString()               },
					{ "steamid",     player.GetParsedUserID()                 },
					{ "class",       player.Role.ToString()                   },
					{ "team",        player.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.onstopcountdown.default", variables);
			}
		}

		[PluginEvent(ServerEventType.WarheadDetonation)]
		public void OnDetonate()
		{
			plugin.SendMessage("messages.ondetonate");
		}

		[PluginEvent(ServerEventType.LczDecontaminationStart)]
		public void OnDecontaminate()
		{
			plugin.SendMessage("messages.ondecontaminate");
		}

		/* TODO: [PluginEvent(ServerEventType)]
		public void OnSummonVehicle(SummonVehicleEvent ev)
		{
			if (ev.IsCI)
			{
				this.plugin.SendMessage(Config.GetArray("messages.onsummonvehicle.chaos"), "messages.onsummonvehicle.chaos");
			}
			else
			{
				this.plugin.SendMessage(Config.GetArray("messages.onsummonvehicle.mtf"), "messages.onsummonvehicle.mtf");
			}
		}
		*/

		/*
		public void OnPlayerTriggerTesla(PlayerTriggerTeslaEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",              ev.Player.IPAddress                 },
				{ "name",                   ev.Player.Name                      },
				{ "playerid",               ev.Player.PlayerID.ToString()       },
				{ "steamid",                ev.Player.GetParsedUserID() ?? ev.Player.UserID },
				{ "class",                  ev.Player.Role.ToString()    },
				{ "team",                   ev.Player.ReferenceHub.GetTeam().ToString()  }
			};

			if (ev.Triggerable)
			{
				plugin.SendMessage(Config.GetArray("messages.onplayertriggertesla.default"), "messages.onplayertriggertesla.default", variables);
			}
			else
			{
				plugin.SendMessage(Config.GetArray("messages.onplayertriggertesla.ignored"), "messages.onplayertriggertesla.ignored", variables);
			}
		}
		*/

		[PluginEvent(ServerEventType.GeneratorActivated)]
		public void OnGeneratorFinish(Scp079Generator generator)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				//{ "room",           ev.Generator.Room.RoomType.ToString()            },
				//{ "ipaddress",   player.IpAddress                         },
				//{ "name",        player.Nickname                          },
				//{ "playerid",    player.PlayerId.ToString()               },
				//{ "steamid",     player.GetParsedUserID()                 },
				//{ "class",       player.Role.ToString()                   },
				//{ "team",        player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.ongeneratorfinish", variables);
		}

		[PluginEvent(ServerEventType.PlayerUnlockGenerator)]
        public void OnGeneratorUnlock(Player player, Scp079Generator generator)
        {
        	Dictionary<string, string> variables = new Dictionary<string, string>
        	{
        		{ "engaged",                    generator.Engaged.ToString()              },
        		{ "activating",                 generator.Activating.ToString()           },
        		//{ "locked",                     (!generator.).ToString()                  },
        		//{ "open",                       generator.IsOpen.ToString()               },
        		{ "room",                       generator.GetComponentInParent<RoomIdentifier>().Name.ToString() },
        		//{ "starttime",                  generator.ActivationTime.ToString()       },
        		//{ "timeleft",                   generator.ActivationTimeLeft.ToString()   },
        		{ "ipaddress",                  player.IpAddress                          },
        		{ "name",                       player.Nickname                           },
        		{ "playerid",                   player.PlayerId.ToString()                },
        		{ "steamid",                    player.GetParsedUserID()                  },
        		{ "class",                      player.Role.ToString()                    },
        		{ "team",                       player.ReferenceHub.GetTeam().ToString()  }
        	};
        	plugin.SendMessage("messages.ongeneratorunlock", variables);
        }

        [PluginEvent(ServerEventType.PlayerOpenGenerator)]
        public void OnGeneratorOpen(Player player, Scp079Generator generator)
        {
        	Dictionary<string, string> variables = new Dictionary<string, string>
        	{
        		{ "engaged",          generator.Engaged.ToString()                 },
        		{ "activating",       generator.Activating.ToString()              },
        		//{ "locked",           (!ev.Generator.IsUnlocked).ToString()        },
        		//{ "open",             generator.IsOpen.ToString()                  },
        		{ "room",             generator.GetComponentInParent<RoomIdentifier>().Name.ToString() },
        		//{ "starttime",        ev.Generator.ActivationTime.ToString()       },
        		//{ "timeleft",         ev.Generator.ActivationTimeLeft.ToString()   },
        		{ "ipaddress",        player.IpAddress                             },
        		{ "name",             player.Nickname                              },
        		{ "playerid",         player.PlayerId.ToString()                   },
        		{ "steamid",          player.GetParsedUserID()                     },
        		{ "class",            player.Role.ToString()                       },
        		{ "team",             player.ReferenceHub.GetTeam().ToString()     }
        	};

        	plugin.SendMessage("messages.ongeneratoropen", variables);
        }

        [PluginEvent(ServerEventType.PlayerCloseGenerator)]
        public void OnGeneratorClose(Player player, Scp079Generator generator)
        {
        	Dictionary<string, string> variables = new Dictionary<string, string>
        	{
        		{ "engaged",        generator.Engaged.ToString()                  },
        		{ "activating",     generator.Activating.ToString()               },
        		//{ "locked",         (!ev.Generator.IsUnlocked).ToString()         },
        		//{ "open",           generator.IsOpen.ToString()                   },
        		{ "room",           generator.GetComponentInParent<RoomIdentifier>().Name.ToString() },
        		//{ "starttime",      ev.Generator.ActivationTime.ToString()        },
        		//{ "timeleft",       ev.Generator.ActivationTimeLeft.ToString()    },
        		{ "ipaddress",      player.IpAddress                              },
        		{ "name",           player.Nickname                               },
        		{ "playerid",       player.PlayerId.ToString()                    },
        		{ "steamid",        player.GetParsedUserID()                      },
        		{ "class",          player.Role.ToString()                        },
        		{ "team",           player.ReferenceHub.GetTeam().ToString()      }
        	};
        	plugin.SendMessage("messages.ongeneratorclose", variables);
        }

        [PluginEvent(ServerEventType.PlayerActivateGenerator)]
        public void OnGeneratorActivated(Player player, Scp079Generator generator)
        {
        	Dictionary<string, string> variables = new Dictionary<string, string>
        	{
        		{ "engaged",       generator.Engaged.ToString()                },
        		{ "activating",    generator.Activating.ToString()             },
        		//{ "locked",        (!ev.Generator.IsUnlocked).ToString()       },
        		//{ "open",          ev.Generator.IsOpen.ToString()              },
        		{ "room",          generator.GetComponentInParent<RoomIdentifier>().Name.ToString() },
        		//{ "starttime",     ev.Generator.ActivationTime.ToString()      },
        		//{ "timeleft",      ev.Generator.ActivationTimeLeft.ToString()  },
        		{ "ipaddress",     player.IpAddress                            },
        		{ "name",          player.Nickname                             },
        		{ "playerid",      player.PlayerId.ToString()                  },
        		{ "steamid",       player.GetParsedUserID()                    },
        		{ "class",         player.Role.ToString()                      },
        		{ "team",          player.ReferenceHub.GetTeam().ToString()    }
        	};
        	plugin.SendMessage("messages.ongeneratoractivated", variables);
        }

        [PluginEvent(ServerEventType.PlayerDeactivatedGenerator)]
        public void OnGeneratorDeactivated(Player player, Scp079Generator generator)
        {
        	Dictionary<string, string> variables = new Dictionary<string, string>
        	{
        		{ "engaged",       generator.Engaged.ToString()                },
        		{ "activating",    generator.Activating.ToString()             },
        		//{ "locked",        (!ev.Generator.IsUnlocked).ToString()       },
        		//{ "open",          ev.Generator.IsOpen.ToString()              },
        		{ "room",          generator.GetComponentInParent<RoomIdentifier>().Name.ToString() },
        		//{ "starttime",     ev.Generator.ActivationTime.ToString()      },
        		//{ "timeleft",      ev.Generator.ActivationTimeLeft.ToString()  },
        		{ "ipaddress",     player.IpAddress                            },
        		{ "name",          player.Nickname                             },
        		{ "playerid",      player.PlayerId.ToString()                  },
        		{ "steamid",       player.GetParsedUserID()                    },
        		{ "class",         player.Role.ToString()                      },
        		{ "team",          player.ReferenceHub.GetTeam().ToString()    }
        	};
        	plugin.SendMessage("messages.ongeneratordeactivated", variables);
        }
	}
}