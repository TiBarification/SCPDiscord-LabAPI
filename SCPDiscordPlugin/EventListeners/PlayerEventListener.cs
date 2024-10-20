using System.Collections.Generic;
using System.Linq;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using Respawning;

namespace SCPDiscord.EventListeners
{
  internal class PlayerEventListener
  {
    private readonly SCPDiscord plugin;

    // First dimension is target player second dimension is attacking player
    private static readonly Dictionary<Team, Team> teamKillingMatrix = new Dictionary<Team, Team>
    {
      { Team.FoundationForces, Team.Scientists       },
      { Team.ChaosInsurgency,  Team.ClassD           },
      { Team.Scientists,       Team.FoundationForces },
      { Team.ClassD,           Team.ChaosInsurgency  }
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

    // Convert damage handler to smod style damage type
    private string GetDamageType(DamageHandlerBase handler)
    {
      switch (handler)
      {
        case DisruptorDamageHandler _:
          return "disruptor";

        case ExplosionDamageHandler _:
          return "an explosion";

        case FirearmDamageHandler firearmDamageHandler:
          return firearmDamageHandler.WeaponType.ToString();

        case JailbirdDamageHandler _:
          return "a jailbird";

        case MicroHidDamageHandler _:
          return "a Micro-HID";

        case RecontainmentDamageHandler _:
          return "recontainment";

        case Scp018DamageHandler _:
          return "SCP-018";

        case Scp049DamageHandler _:
          return "SCP-049";

        case Scp096DamageHandler _:
          return "SCP-096";

        case ScpDamageHandler _:
          return "SCP attack";

        case Scp3114DamageHandler _:
          return "SCP-3114";

        case Scp939DamageHandler _:
          return "SCP-939";

        //case AttackerDamageHandler attackerDamageHandler:
        //	break;

        case CustomReasonDamageHandler _:
          return "UNKNOWN";

        case UniversalDamageHandler _:
          return "UNKNOWN";

        case WarheadDamageHandler _:
          return "alpha warhead";

        //case StandardDamageHandler standardDamageHandler:
        //	break;

        default:
          return "UNKNOWN";
      }
    }


    [PluginEvent]
    public void OnPlayerHurt(PlayerDamageEvent ev)
    {
      if (ev.Target == null || ev.Target.Role == RoleTypeId.None || !(ev.DamageHandler is StandardDamageHandler stdHandler))
      {
        return;
      }

      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "damage",     stdHandler.Damage.ToString("0.##") },
        { "damagetype", GetDamageType(ev.DamageHandler)    }
      };

      if (ev.Player == null || ev.Target.PlayerId == ev.Player.PlayerId)
      {
        variables.AddPlayerVariables(ev.Target, "target");

        SCPDiscord.SendMessage("messages.onplayerhurt.noattacker", variables);
      }
      else
      {
        variables.AddPlayerVariables(ev.Target, "target");
        variables.AddPlayerVariables(ev.Player, "attacker");

        if (IsTeamDamage(ev.Player.ReferenceHub.GetTeam(), ev.Target.ReferenceHub.GetTeam()))
        {
          SCPDiscord.SendMessage("messages.onplayerhurt.friendlyfire", variables);
          return;
        }

        SCPDiscord.SendMessage("messages.onplayerhurt.default", variables);
      }
    }

    [PluginEvent]
    public void OnPlayerDie(PlayerDyingEvent ev)
    {
      if (ev.Player == null || ev.Player.Role == RoleTypeId.None || !(ev.DamageHandler is StandardDamageHandler))
      {
        return;
      }

      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "damagetype", GetDamageType(ev.DamageHandler) }
      };

      if (ev.Attacker == null || ev.Player.PlayerId == ev.Attacker.PlayerId)
      {
        variables.AddPlayerVariables(ev.Player, "target");
        SCPDiscord.SendMessage("messages.onplayerdie.nokiller", variables);
      }
      else
      {
        variables.AddPlayerVariables(ev.Attacker, "attacker");
        variables.AddPlayerVariables(ev.Player, "target");

        if (IsTeamDamage(ev.Attacker.ReferenceHub.GetTeam(), ev.Player.ReferenceHub.GetTeam()))
        {
          SCPDiscord.SendMessage("messages.onplayerdie.friendlyfire", variables);
        }
        else
        {
          SCPDiscord.SendMessage("messages.onplayerdie.default", variables);
        }
      }
    }

    [PluginEvent]
    public void OnPlayerPickupAmmo(PlayerPickupAmmoEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "ammo", ev.Item.Info.ItemId.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");

      SCPDiscord.SendMessage("messages.onplayerpickupammo", variables);
    }

    [PluginEvent]
    public void OnPlayerPickupArmor(PlayerPickupArmorEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "armor", ev.Item.Info.ItemId.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerpickuparmor", variables);
    }

    [PluginEvent]
    public void OnPlayerPickupSCP330(PlayerPickupScp330Event ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>();
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerpickupscp330", variables);
    }

    [PluginEvent]
    public void OnPlayerPickupItem(PlayerSearchedPickupEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "item", ev.Item.Info.ItemId.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerpickupitem", variables);
    }

    [PluginEvent]
    public void OnPlayerDropAmmo(PlayerDropAmmoEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "ammo",   ev.Item.ToString()   },
        { "amount", ev.Amount.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerdropammo", variables);
    }

    [PluginEvent]
    public void OnPlayerDropItem(PlayerDropItemEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "item", ev.Item.ItemTypeId.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerdropitem", variables);
    }

    [PluginEvent]
    public void OnPlayerJoin(PlayerJoinedEvent ev)
    {
      if (ev.Player.PlayerId == Server.Instance.PlayerId)
      {
        return;
      }

      Dictionary<string, string> variables = new Dictionary<string, string>();
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerjoin", variables);
    }

    [PluginEvent]
    public void OnPlayerLeave(PlayerLeftEvent ev)
    {
      if (ev.Player?.PlayerId == Server.Instance.PlayerId || ev.Player?.UserId == null)
      {
        return;
      }

      Dictionary<string, string> variables = new Dictionary<string, string>();
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerleave", variables);
    }

    [PluginEvent]
    public void OnSpawn(PlayerSpawnEvent ev)
    {
      if (ev.Player == null
          || ev.Player.UserId == "server"
          || ev.Player.UserId == null
          || ev.Role == RoleTypeId.None
          || ev.Role == RoleTypeId.Spectator
          || ev.Role == RoleTypeId.Overwatch)
      {
        return;
      }

      Dictionary<string, string> variables = new Dictionary<string, string>();
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onspawn", variables);
    }

    [PluginEvent]
    public void OnTeamRespawn(TeamRespawnEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "players", ev.Players.Select(x => x.Nickname).ToString() }
      };
      SCPDiscord.SendMessage(ev.Team == SpawnableTeamType.ChaosInsurgency ? "messages.onteamrespawn.ci" : "messages.onteamrespawn.mtf", variables);
    }

    [PluginEvent]
    public void OnThrowProjectile(PlayerThrowProjectileEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "type", ev.Item.ItemTypeId.ToString() }
      };
      variables.AddPlayerVariables(ev.Thrower, "player");
      SCPDiscord.SendMessage("messages.onthrowprojectile", variables);
    }

    [PluginEvent]
    public void OnItemUse(PlayerUsedItemEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "item", ev.Item.ItemTypeId.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onitemuse", variables);
    }

    [PluginEvent]
    public void OnHandcuffed(PlayerHandcuffEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>();
      if (ev.Player == null)
      {
        variables.AddPlayerVariables(ev.Target, "target");
        SCPDiscord.SendMessage("messages.onhandcuff.nootherplayer", variables);
      }
      else
      {
        variables.AddPlayerVariables(ev.Target, "target");
        variables.AddPlayerVariables(ev.Player, "disarmer");
        SCPDiscord.SendMessage("messages.onhandcuff.default", variables);
      }
    }

    [PluginEvent]
    public void OnHandcuffsRemoved(PlayerRemoveHandcuffsEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>();
      if (ev.Player != null)
      {
        variables.AddPlayerVariables(ev.Target, "target");
        variables.AddPlayerVariables(ev.Player, "disarmer");
        SCPDiscord.SendMessage("messages.onhandcuffremoved.default", variables);
      }
      else
      {
        variables.AddPlayerVariables(ev.Target, "target");
        SCPDiscord.SendMessage("messages.onhandcuffremoved.nootherplayer", variables);
      }
    }

    [PluginEvent]
    public void OnReload(PlayerReloadWeaponEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "weapon",      ev.Firearm.ItemTypeId.ToString()                },
        { "maxclipsize", ev.Firearm.AmmoManagerModule.MaxAmmo.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onreload", variables);
    }

    [PluginEvent]
    public void OnGrenadeExplosion(GrenadeExplodedEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "type", ev?.Grenade.Info.ItemId.ToString() }
      };

      if (ev?.Thrower.Hub != null)
      {
        variables.AddPlayerVariables(new Player(ev.Thrower.Hub), "player");
      }

      SCPDiscord.SendMessage("messages.ongrenadeexplosion", variables);
    }

    [PluginEvent]
    public void OnPlayerEscape(PlayerEscapeEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "newrole", ev.NewRole.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerescape", variables);
    }

    [PluginEvent]
    public void OnPlayerReceiveEffect(PlayerReceiveEffectEvent ev)
    {
      Dictionary<string, string> variables = new Dictionary<string, string>
      {
        { "effect", ev.Effect.ToString() },
        { "duration", ev.Duration.ToString() },
        { "intensity", ev.Intensity.ToString() }
      };
      variables.AddPlayerVariables(ev.Player, "player");
      SCPDiscord.SendMessage("messages.onplayerreceiveeffect", variables);
    }
  }
}