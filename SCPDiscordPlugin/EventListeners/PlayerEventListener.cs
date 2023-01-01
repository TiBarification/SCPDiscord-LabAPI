using System.Collections.Generic;
using CommandSystem;
using Footprinting;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using MapGeneration;
using MapGeneration.Distributors;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using RemoteAdmin;
using Respawning;
using Scp914;
using UnityEngine;

namespace SCPDiscord.EventListeners
{
	internal class PlayerEventListener
	{
		private readonly SCPDiscord plugin;

		// First dimension is target player second dimension is attacking player
		private static readonly Dictionary<Team, Team> teamKillingMatrix = new Dictionary<Team, Team>
		{
			{ Team.FoundationForces, Team.Scientists },
			{ Team.ChaosInsurgency, Team.ClassD },
			{ Team.Scientists, Team.FoundationForces },
			{ Team.ClassD, Team.ChaosInsurgency }
		};

		public PlayerEventListener(SCPDiscord pl)
		{
			plugin = pl;
		}

		private bool IsTeamDamage(Team attackerTeam, Team targetTeam)
		{
			if (!plugin.roundStarted)
			{
				return false;
			}
			if (attackerTeam == targetTeam)
			{
				return true;
			}
			foreach (KeyValuePair<Team, Team> team in teamKillingMatrix)
			{
				if (attackerTeam == team.Value && targetTeam == team.Key)
				{
					return true;
				}
			}
			return false;
		}

		public enum DamageType
		{
			NONE = -1, // 0xFFFFFFFF
			RECONTAINED = 0,
			WARHEAD = 1,
			SCP_049 = 2,
			UNKNOWN = 3,
			ASPHYXIATED = 4,
			BLEEDING = 5,
			FALLING = 6,
			POCKET_DECAY = 7,
			DECONTAMINATION = 8,
			POISON = 9,
			SCP_207 = 10, // 0x0000000A
			SEVERED_HANDS = 11, // 0x0000000B
			MICRO_HID = 12, // 0x0000000C
			TESLA = 13, // 0x0000000D
			EXPLOSION = 14, // 0x0000000E
			SCP_096 = 15, // 0x0000000F
			SCP_173 = 16, // 0x00000010
			SCP_939 = 17, // 0x00000011
			SCP_049_2 = 18, // 0x00000012
			UNKNOWN_FIREARM = 19, // 0x00000013
			CRUSHED = 20, // 0x00000014
			FEMUR_BREAKER = 21, // 0x00000015
			FRIENDLY_FIRE_PUNISHMENT = 22, // 0x00000016
			HYPOTHERMIA = 23, // 0x00000017
			SCP_106 = 24, // 0x00000018
			SCP_018 = 25, // 0x00000019
			COM15 = 26, // 0x0000001A
			E11_SR = 27, // 0x0000001B
			CROSSVEC = 28, // 0x0000001C
			FSP9 = 29, // 0x0000001D
			LOGICER = 30, // 0x0000001E
			COM18 = 31, // 0x0000001F
			REVOLVER = 32, // 0x00000020
			AK = 33, // 0x00000021
			SHOTGUN = 34, // 0x00000022
			DISRUPTOR = 35, // 0x00000023
		}

		// Convert damage handler to smod style damage type
		private string GetDamageType(DamageHandlerBase handler)
		{
			switch (handler)
			{
				case DisruptorDamageHandler _:
					return "DISRUPTOR";

				case ExplosionDamageHandler explosionDamageHandler:
					return "EXPLOSION";

				case FirearmDamageHandler firearmDamageHandler:
					return firearmDamageHandler.WeaponType.ToString();

				case MicroHidDamageHandler microHidDamageHandler:
					return "MICRO_HID";

				case RecontainmentDamageHandler recontainmentDamageHandler:
					return "RECONTAINED";

				case Scp018DamageHandler scp018DamageHandler:
					return "SCP_018";

				case Scp049DamageHandler scp049DamageHandler:
					return "SCP_049";

				case Scp096DamageHandler scp096DamageHandler:
					return "SCP_096";

				case ScpDamageHandler scpDamageHandler:
					return "SCP_ATTACK";

				case Scp939DamageHandler scp939DamageHandler:
					return "SCP_939";

				//case AttackerDamageHandler attackerDamageHandler:
				//	break;

				case CustomReasonDamageHandler customReasonDamageHandler:
					return "UNKNOWN";

				case UniversalDamageHandler universalDamageHandler:
					return "UNKNOWN";

				case WarheadDamageHandler warheadDamageHandler:
					return "WARHEAD";

				//case StandardDamageHandler standardDamageHandler:
				//	break;

				default:
					return "UNKNOWN";
			}
		}

		[PluginEvent(ServerEventType.PlayerDamage)]
		public void OnPlayerHurt(Player target, Player attacker, DamageHandlerBase damageHandler)
		{
			if (target == null || target.Role == RoleTypeId.None || !(damageHandler is StandardDamageHandler stdHandler))
			{
				return;
			}

			if (attacker == null || target.PlayerId == attacker.PlayerId)
			{
				Dictionary<string, string> noAttackerVar = new Dictionary<string, string>
				{
					{ "damage",             stdHandler.Damage.ToString()             },
					{ "damagetype",         GetDamageType(damageHandler)             },
					{ "playeripaddress",    target.IpAddress                         },
					{ "playername",         target.Nickname                          },
					{ "playerplayerid",     target.PlayerId.ToString()               },
					{ "playersteamid",      target.GetParsedUserID()                 },
					{ "playerclass",        target.Role.ToString()                   },
					{ "playerteam",         target.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.onplayerhurt.noattacker", noAttackerVar);
				return;
			}

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "damage",             stdHandler.Damage.ToString()               },
				{ "damagetype",         GetDamageType(damageHandler)               },
				{ "attackeripaddress",  attacker.IpAddress                         },
				{ "attackername",       attacker.Nickname                          },
				{ "attackerplayerid",   attacker.PlayerId.ToString()               },
				{ "attackersteamid",    attacker.GetParsedUserID()                 },
				{ "attackerclass",      attacker.Role.ToString()                   },
				{ "attackerteam",       attacker.ReferenceHub.GetTeam().ToString() },
				{ "playeripaddress",    target.IpAddress                           },
				{ "playername",         target.Nickname                            },
				{ "playerplayerid",     target.PlayerId.ToString()                 },
				{ "playersteamid",      target.GetParsedUserID()                   },
				{ "playerclass",        target.Role.ToString()                     },
				{ "playerteam",         target.ReferenceHub.GetTeam().ToString()   }
			};

			if (IsTeamDamage(attacker.ReferenceHub.GetTeam(), target.ReferenceHub.GetTeam()))
			{
				plugin.SendMessage("messages.onplayerhurt.friendlyfire", variables);
				return;
			}

			plugin.SendMessage("messages.onplayerhurt.default", variables);
		}

		[PluginEvent(ServerEventType.PlayerDeath)]
		public void OnPlayerDie(Player target, Player attacker, DamageHandlerBase damageHandler)
		{
			if (target == null || target.Role == RoleTypeId.None || !(damageHandler is StandardDamageHandler stdHandler))
			{
				return;
			}

			if (attacker == null || target.PlayerId == attacker.PlayerId)
			{
				Dictionary<string, string> noKillerVar = new Dictionary<string, string>
				{
					{ "damagetype",         GetDamageType(damageHandler)         },
					{ "playeripaddress",    target.IpAddress                 },
					{ "playername",         target.Nickname                      },
					{ "playerplayerid",     target.PlayerId.ToString()       },
					{ "playersteamid",      target.GetParsedUserID()         },
					{ "playerclass",        target.Role.ToString()  },
					{ "playerteam",         target.ReferenceHub.GetTeam().ToString()  }
				};
				plugin.SendMessage("messages.onplayerdie.nokiller", noKillerVar);
				return;
			}

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "damagetype",         GetDamageType(damageHandler)         },
				{ "attackeripaddress",  attacker.IpAddress                 },
				{ "attackername",       attacker.Nickname                      },
				{ "attackerplayerid",   attacker.PlayerId.ToString()       },
				{ "attackersteamid",    attacker.GetParsedUserID()         },
				{ "attackerclass",      attacker.Role.ToString()  },
				{ "attackerteam",       attacker.ReferenceHub.GetTeam().ToString()  },
				{ "playeripaddress",    target.IpAddress                 },
				{ "playername",         target.Nickname                      },
				{ "playerplayerid",     target.PlayerId.ToString()       },
				{ "playersteamid",      target.GetParsedUserID()         },
				{ "playerclass",        target.Role.ToString()  },
				{ "playerteam",         target.ReferenceHub.GetTeam().ToString()  }
			};

			if (IsTeamDamage(attacker.ReferenceHub.GetTeam(), target.ReferenceHub.GetTeam()))
			{
				plugin.SendMessage("messages.onplayerdie.friendlyfire", variables);
				return;
			}
			plugin.SendMessage("messages.onplayerdie.default", variables);
		}

		[PluginEvent(ServerEventType.PlayerPickupAmmo)]
		public void OnPlayerPickupAmmo(Player player, ItemPickupBase ammo)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ammo",         ammo.ToString()                    },
				{ "ipaddress",    player.IpAddress                   },
				{ "name",         player.Nickname                        },
				{ "playerid",     player.PlayerId.ToString()         },
				{ "steamid",      player.GetParsedUserID()                     },
				{ "class",        player.Role.ToString()    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()    }
			};
			plugin.SendMessage("messages.onplayerpickupammo", variables);
		}

		[PluginEvent(ServerEventType.PlayerPickupArmor)]
		public void OnPlayerPickupArmor(Player player, ItemPickupBase armor)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "armor",         armor.ToString()                    },
				{ "ipaddress",    player.IpAddress                   },
				{ "name",         player.Nickname                        },
				{ "playerid",     player.PlayerId.ToString()         },
				{ "steamid",      player.GetParsedUserID()                     },
				{ "class",        player.Role.ToString()    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()    }
			};
			plugin.SendMessage("messages.onplayerpickuparmor", variables);
		}

		[PluginEvent(ServerEventType.PlayerPickupScp330)]
		public void OnPlayerPickupSCP330(Player player, ItemPickupBase scp330)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",    player.IpAddress                   },
				{ "name",         player.Nickname                        },
				{ "playerid",     player.PlayerId.ToString()         },
				{ "steamid",      player.GetParsedUserID()                     },
				{ "class",        player.Role.ToString()    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()    }
			};
			plugin.SendMessage("messages.onplayerpickupscp330", variables);
		}

		[PluginEvent(ServerEventType.PlayerSearchedPickup)]
		public void OnPlayerPickupItem(Player player, ItemPickupBase item)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "item",         item.ToString()                    },
				{ "ipaddress",    player.IpAddress                   },
				{ "name",         player.Nickname                        },
				{ "playerid",     player.PlayerId.ToString()         },
				{ "steamid",      player.GetParsedUserID()                     },
				{ "class",        player.Role.ToString()    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()    }
			};
			plugin.SendMessage("messages.onplayerpickupitem", variables);
		}

		[PluginEvent(ServerEventType.PlayerDropAmmo)]
		public void OnPlayerDropAmmo(Player player, ItemType ammo, int amount)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ammo",         ammo.ToString()                    },
				{ "amount",         amount.ToString()                    },
				{ "ipaddress",    player.IpAddress                   },
				{ "name",         player.Nickname                        },
				{ "playerid",     player.PlayerId.ToString()         },
				{ "steamid",      player.GetParsedUserID() },
				{ "class",        player.Role.ToString()    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()    }
			};
			plugin.SendMessage("messages.onplayerdropammo", variables);
		}

		[PluginEvent(ServerEventType.PlayerDropItem)]
		public void OnPlayerDropItem(Player player, ItemBase item)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "item",         item.name                    },
				{ "ipaddress",    player.IpAddress                   },
				{ "name",         player.Nickname                        },
				{ "playerid",     player.PlayerId.ToString()         },
				{ "steamid",      player.GetParsedUserID() },
				{ "class",        player.Role.ToString()    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()    }
			};
			plugin.SendMessage("messages.onplayerdropitem", variables);
		}

		[PluginEvent(ServerEventType.PlayerJoined)]
		public void OnPlayerJoin(Player player)
		{
			if (player.PlayerId == Server.Instance.PlayerId) return;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",    player.IpAddress                          },
				{ "name",         player.Nickname                           },
				{ "playerid",     player.PlayerId.ToString()                },
				{ "steamid",      player.GetParsedUserID()                  },
				{ "class",        player.Role.ToString()                    },
				{ "team",         player.ReferenceHub.GetTeam().ToString()  }
			};
			plugin.SendMessage("messages.onplayerjoin", variables);
		}

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void OnPlayerLeave(Player player)
		{
			if (player.PlayerId == Server.Instance.PlayerId) return;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress", player.IpAddress           },
				{ "name", player.Nickname                 },
				{ "steamid", player.GetParsedUserID()     },
				{ "playerid", player.PlayerId.ToString()  }
			};
			this.plugin.SendMessage("messages.onplayerleave", variables);
		}

		/*
		public void OnNicknameSet(PlayerNicknameSetEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "nickname",       ev.Nickname                         },
				{ "ipaddress",      ev.Player.IPAddress                 },
				{ "name",           ev.Player.Name                      },
				{ "playerid",       ev.Player.PlayerID.ToString()       },
				{ "steamid",        ev.Player.GetParsedUserID() ?? ev.Player.UserID },
				{ "class",          ev.Player.Role.ToString()  },
				{ "team",           ev.Player.ReferenceHub.GetTeam().ToString()  }
			};
			plugin.SendMessage(Config.GetArray("messages.onnicknameset"), "messages.onnicknameset", variables);
		}
		*/

		[PluginEvent(ServerEventType.PlayerChangeRole)]
		public void OnSetRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason changeReason)
		{
			if (newRole == RoleTypeId.None)
			{
				return;
			}

			// TODO: Split into different reasons
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",      player.IpAddress                          },
				{ "name",           player.Nickname                           },
				{ "playerid",       player.PlayerId.ToString()                },
				{ "steamid",        player.GetParsedUserID()                  },
				{ "class",          player.Role.ToString()                    },
				{ "team",           player.ReferenceHub.GetTeam().ToString()  }
			};
			plugin.SendMessage("messages.onsetrole", variables);
		}

		[PluginEvent(ServerEventType.PlayerSpawn)]
		public void OnSpawn(Player player, RoleTypeId role)
		{
			if (player == null || player.UserId == "server" || role == RoleTypeId.None) return;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          role.ToString()                          },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};

			plugin.SendMessage("messages.onspawn", variables);
		}

		[PluginEvent(ServerEventType.TeamRespawn)]
		public void OnTeamRespawn(SpawnableTeamType team)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				//{ "players",    ev.PlayerList.ToString()    }
			};

			plugin.SendMessage(team == SpawnableTeamType.ChaosInsurgency ? "messages.onteamrespawn.ci" : "messages.onteamrespawn.mtf", variables);
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

		[PluginEvent(ServerEventType.PlayerThrowProjectile)]
		public void OnThrowProjectile(Player player, ThrowableItem item, ThrowableItem.ProjectileSettings settings, bool fullForce)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "type",           item.ItemTypeId.ToString()               },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onthrowprojectile", variables);
		}

		/*
		public void OnPlayerInfected(PlayerInfectedEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "damage",                 ev.Damage.ToString()                    },
				{ "infecttime",             ev.InfectTime.ToString()                },
				{ "attackeripaddress",      ev.Attacker.IPAddress                   },
				{ "attackername",           ev.Attacker.Name                        },
				{ "attackerplayerid",       ev.Attacker.PlayerID.ToString()         },
				{ "attackersteamid",        ev.Attacker.GetParsedUserID() ?? ev.Player.UserID },
				{ "attackerclass",          ev.Attacker.Role.ToString()      },
				{ "attackerteam",           ev.Attacker.ReferenceHub.GetTeam().ToString()    },
				{ "playeripaddress",        ev.Attacker.IPAddress                   },
				{ "playername",             ev.Player.Name                          },
				{ "playerplayerid",         ev.Player.PlayerID.ToString()           },
				{ "playersteamid",          ev.Player.GetParsedUserID() ?? ev.Player.UserID },
				{ "playerclass",            ev.Player.Role.ToString()        },
				{ "playerteam",             ev.Player.ReferenceHub.GetTeam().ToString()      }
			};
			plugin.SendMessage(Config.GetArray("messages.onplayerinfected"), "messages.onplayerinfected", variables);
		}
		*/

		[PluginEvent(ServerEventType.RagdollSpawn)]
		public void OnSpawnRagdoll(Player player, IRagdollRole ragdollRole, DamageHandlerBase damageHandler)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "damagetype",         GetDamageType(damageHandler)              },
				{ "ipaddress",          player.IpAddress                          },
				{ "name",               player.Nickname                           },
				{ "playerid",           player.PlayerId.ToString()                },
				{ "steamid",            player.GetParsedUserID()                  },
				{ "class",              player.ToString()                         },
				{ "team",               player.ReferenceHub.GetTeam().ToString()  }
			};
			plugin.SendMessage("messages.onspawnragdoll", variables);
		}

		[PluginEvent(ServerEventType.PlayerUseItem)]
		public void OnItemUse(Player player, UsableItem item)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "item",           item.ItemTypeId.ToString()               },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onitemuse", variables);
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

		[PluginEvent(ServerEventType.PlayerHandcuff)]
		public void OnHandcuffed(Player disarmer, Player target)
		{
			if (disarmer != null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "targetipaddress",    target.IpAddress                           },
					{ "targetname",         target.Nickname                            },
					{ "targetplayerid",     target.PlayerId.ToString()                 },
					{ "targetsteamid",      target.GetParsedUserID()                   },
					{ "targetclass",        target.Role.ToString()                     },
					{ "targetteam",         target.ReferenceHub.GetTeam().ToString()   },
					{ "playeripaddress",    disarmer.IpAddress                         },
					{ "playername",         disarmer.Nickname                          },
					{ "playerplayerid",     disarmer.PlayerId.ToString()               },
					{ "playersteamid",      disarmer.GetParsedUserID()                 },
					{ "playerclass",        disarmer.Role.ToString()                   },
					{ "playerteam",         disarmer.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.onhandcuff.default", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "targetipaddress",    target.IpAddress                           },
					{ "targetname",         target.Nickname                            },
					{ "targetplayerid",     target.PlayerId.ToString()                 },
					{ "targetsteamid",      target.GetParsedUserID()                   },
					{ "targetclass",        target.Role.ToString()                     },
					{ "targetteam",         target.ReferenceHub.GetTeam().ToString()   },
				};
				plugin.SendMessage("messages.onhandcuff.nootherplayer", variables);
			}
		}

		[PluginEvent(ServerEventType.PlayerRemoveHandcuffs)]
		public void OnHandcuffsRemoved(Player disarmer, Player target)
		{
			if (disarmer != null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "targetipaddress",    target.IpAddress                           },
					{ "targetname",         target.Nickname                            },
					{ "targetplayerid",     target.PlayerId.ToString()                 },
					{ "targetsteamid",      target.GetParsedUserID()                   },
					{ "targetclass",        target.Role.ToString()                     },
					{ "targetteam",         target.ReferenceHub.GetTeam().ToString()   },
					{ "playeripaddress",    disarmer.IpAddress                         },
					{ "playername",         disarmer.Nickname                          },
					{ "playerplayerid",     disarmer.PlayerId.ToString()               },
					{ "playersteamid",      disarmer.GetParsedUserID()                 },
					{ "playerclass",        disarmer.Role.ToString()                   },
					{ "playerteam",         disarmer.ReferenceHub.GetTeam().ToString() }
				};
				plugin.SendMessage("messages.onhandcuff.default", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "targetipaddress",    target.IpAddress                           },
					{ "targetname",         target.Nickname                            },
					{ "targetplayerid",     target.PlayerId.ToString()                 },
					{ "targetsteamid",      target.GetParsedUserID()                   },
					{ "targetclass",        target.Role.ToString()                     },
					{ "targetteam",         target.ReferenceHub.GetTeam().ToString()   },
				};
				plugin.SendMessage("messages.onhandcuff.nootherplayer", variables);
			}
		}

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

		[PluginEvent(ServerEventType.PlayerChangeRadioRange)]
		public void OnPlayerRadioSwitch(Player player, RadioItem radio, byte range)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "setting",        radio.RangeLevel.ToString()              },
				{ "ipaddress",      player.IpAddress                         },
				{ "name",           player.Nickname                          },
				{ "playerid",       player.PlayerId.ToString()               },
				{ "steamid",        player.GetParsedUserID()                 },
				{ "class",          player.Role.ToString()                   },
				{ "team",           player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onplayerradioswitch", variables);
		}

		[PluginEvent(ServerEventType.Scp049ResurrectBody)]
		public void OnRecallZombie(Player player, Player target, BasicRagdoll ragdoll)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "targetipaddress",    target.IpAddress                         },
				{ "targetname",         target.Nickname                          },
				{ "targetplayerid",     target.PlayerId.ToString()               },
				{ "targetsteamid",      target.GetParsedUserID()                 },
				{ "targetclass",        target.Role.ToString()                   },
				{ "targetteam",         target.ReferenceHub.GetTeam().ToString() },
				{ "playeripaddress",    player.IpAddress                         },
				{ "playername",         player.Nickname                          },
				{ "playerplayerid",     player.PlayerId.ToString()               },
				{ "playersteamid",      player.GetParsedUserID()                 },
				{ "playerclass",        player.Role.ToString()                   },
				{ "playerteam",         player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.onrecallzombie", variables);
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

		[PluginEvent(ServerEventType.PlayerReloadWeapon)]
		public void OnReload(Player player, Firearm weapon)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "weapon",      weapon.name                                 },
				{ "maxclipsize", weapon.AmmoManagerModule.MaxAmmo.ToString() },
				{ "ipaddress",   player?.IpAddress                           },
				{ "name",        player?.Nickname                            },
				{ "playerid",    player?.PlayerId.ToString()                 },
				{ "steamid",     player?.GetParsedUserID()                   },
				{ "class",       player?.Role.ToString()                     },
				{ "team",        player?.ReferenceHub.GetTeam().ToString()   }
			};
			plugin.SendMessage("messages.onreload", variables);
		}

		[PluginEvent(ServerEventType.GrenadeExploded)]
		public void OnGrenadeExplosion(Footprint footprint, Vector3 position, ItemPickupBase grenade)
		{
			Player player = new Player(footprint.Hub);
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "type",        grenade.name                                },
				{ "ipaddress",   player?.IpAddress                           },
				{ "name",        player?.Nickname                            },
				{ "playerid",    player?.PlayerId.ToString()                 },
				{ "steamid",     player?.GetParsedUserID()                   },
				{ "class",       player?.Role.ToString()                     },
				{ "team",        player?.ReferenceHub.GetTeam().ToString()   }
			};
			plugin.SendMessage("messages.ongrenadeexplosion", variables);
		}

		/*
		public void OnGrenadeHitPlayer(PlayerGrenadeHitPlayer ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "playeripaddress",    ev.Player?.IPAddress                 },
				{ "playername",         ev.Player?.Name                      },
				{ "playerplayerid",     ev.Player?.PlayerID.ToString()       },
				{ "playersteamid",      ev.Player?.GetParsedUserID() ?? ev.Player?.UserID },
				{ "playerclass",        ev.Player?.PlayerRole?.RoleID.ToString()    },
				{ "playerteam",         ev.Player?.PlayerRole?.Team.ToString()  },
				{ "targetipaddress",    ev.Victim?.IPAddress                 },
				{ "targetname",         ev.Victim?.Name                      },
				{ "targetplayerid",     ev.Victim?.PlayerID.ToString()       },
				{ "targetsteamid",      ev.Victim?.GetParsedUserID() ?? ev.Victim?.UserID },
				{ "targetclass",        ev.Victim?.Role.ToString()    },
				{ "targetteam",         ev.Victim?.ReferenceHub.GetTeam().ToString()  },
			};
			plugin.SendMessage(Config.GetArray("messages.ongrenadehitplayer"), "messages.ongrenadehitplayer", variables);
		}
		*/

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

		[PluginEvent(ServerEventType.Scp079LockDoor)]
		public void On079Lock(Player player, DoorVariant door)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "door",          door.name                                },
				//{ "open",          ev.Door.IsOpen.ToString()                },
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};

			plugin.SendMessage("messages.on079lockdoor", variables);
		}

		/*
		public void On079Elevator()
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "apdrain",                    ev.APDrain.ToString()                },
				{ "elevator",                   ev.Elevator.ElevatorType.ToString()  },
				{ "status",                     ev.Elevator.ElevatorStatus.ToString()},
				{ "ipaddress",                  ev.Player.IPAddress                  },
				{ "name",                       ev.Player.Name                       },
				{ "playerid",                   ev.Player.PlayerID.ToString()        },
				{ "steamid",                    ev.Player.GetParsedUserID() ?? ev.Player.UserID },
				{ "class",                      ev.Player.Role.ToString()     },
				{ "team",                       ev.Player.ReferenceHub.GetTeam().ToString()   }
			};
			if (ev.Elevator.ElevatorStatus == ElevatorStatus.DOWN)
			{
				plugin.SendMessage(Config.GetArray("messages.on079elevator.up"), "messages.on079elevator.up", variables);
			}
			else if (ev.Elevator.ElevatorStatus == ElevatorStatus.UP)
			{
				plugin.SendMessage(Config.GetArray("messages.on079elevator.down"), "messages.on079elevator.down", variables);
			}
		}
		*/

		[PluginEvent(ServerEventType.Scp079UseTesla)]
		public void On079TeslaGate(Player player, TeslaGate teslaGate)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.on079teslagate", variables);
		}

		[PluginEvent(ServerEventType.Scp079GainExperience)]
		public void On079AddExp(Player player, int amount, Scp079HudTranslation reason)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "xptype",        reason.ToString()                        },
				{ "amount",        amount.ToString()                        },
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.on079addexp", variables);
		}

		[PluginEvent(ServerEventType.Scp079LevelUpTier)]
		public void On079LevelUp(Player player, int level)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "level",         level.ToString()                         },
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.on079levelup", variables);
		}

		[PluginEvent(ServerEventType.Scp079UnlockDoor)]
		public void On079UnlockDoor(Player player, DoorVariant door)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "doorname",      door.name                                },
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.on079unlockdoor", variables);
		}

		[PluginEvent(ServerEventType.Scp079LockdownRoom)]
		public void On079Lockdown(Player player, RoomIdentifier room)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "room",          room.name                                },
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.on079lockdown", variables);
		}

		[PluginEvent(ServerEventType.Scp079CancelRoomLockdown)]
		public void On079CancelLockdown(Player player, RoomIdentifier room)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "room",          room.name                                },
				{ "ipaddress",     player.IpAddress                         },
				{ "name",          player.Nickname                          },
				{ "playerid",      player.PlayerId.ToString()               },
				{ "steamid",       player.GetParsedUserID()                 },
				{ "class",         player.Role.ToString()                   },
				{ "team",          player.ReferenceHub.GetTeam().ToString() }
			};
			plugin.SendMessage("messages.on079cancellockdown", variables);
		}
	}
}