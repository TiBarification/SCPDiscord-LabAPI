using System.Collections.Generic;
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
	}
}