using System.Collections.Generic;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace SCPDiscord.EventListeners
{
	internal class EnvironmentEventListener
	{
		private readonly SCPDiscord plugin;

		public EnvironmentEventListener(SCPDiscord plugin)
		{
			this.plugin = plugin;
		}

		/// <summary>
		///  This is the event handler for when a SCP914 is activated
		/// </summary>
		/* TODO [PluginEvent(ServerEventType)]
		public void OnSCP914Activate(SCP914ActivateEvent ev)
		{

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "knobsetting",    ev.KnobSetting.ToString()   }
			};
			this.plugin.SendMessage(Config.GetArray("channels.onscp914activate"), "environment.onscp914activate", variables);
		}
		*/

		/// <summary>
		///  This is the event handler for when the warhead starts counting down, isResumed is false if its the initial count down. Note: activator can be null
		/// </summary>
		/* TODO: [PluginEvent(ServerEventType)]
		public void OnStartCountdown(WarheadStartEvent ev)
		{
			if (ev.Activator == null)
			{
				Dictionary<string, string> vars = new Dictionary<string, string>
				{
					{ "isresumed",      ev.IsResumed.ToString()                 },
					{ "timeleft",       ev.TimeLeft.ToString()                  },
					{ "opendoorsafter", ev.OpenDoorsAfter.ToString()            }
				};
				this.plugin.SendMessage(Config.GetArray("channels.onstartcountdown.noplayer"), "environment.onstartcountdown.noplayer", vars);
				return;
			}

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "isresumed",      ev.IsResumed.ToString()                 },
				{ "timeleft",       ev.TimeLeft.ToString()                  },
				{ "opendoorsafter", ev.OpenDoorsAfter.ToString()            },
				{ "ipaddress",      ev.Activator.IPAddress                  },
				{ "name",           ev.Activator.Name                       },
				{ "playerid",       ev.Activator.PlayerID.ToString()        },
				{ "steamid",        ev.Activator.GetParsedUserID()          },
				{ "class",          ev.Activator.PlayerRole.RoleID.ToString()     },
				{ "team",           ev.Activator.PlayerRole.Team.ToString()   }
			};

			if (ev.IsResumed)
			{
				this.plugin.SendMessage(Config.GetArray("channels.onstartcountdown.resumed"), "environment.onstartcountdown.resumed", variables);
			}
			else
			{
				this.plugin.SendMessage(Config.GetArray("channels.onstartcountdown.initiated"), "environment.onstartcountdown.initiated", variables);
			}
		}
		*/

		/// <summary>
		///  This is the event handler for when the warhead stops counting down.
		/// </summary>
		/* TODO: [PluginEvent(ServerEventType)]
		public void OnStopCountdown(WarheadStopEvent ev)
		{
			if (ev.Activator == null)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "timeleft",       ev.TimeLeft.ToString()                  }
				};
				this.plugin.SendMessage(Config.GetArray("channels.onstopcountdown.noplayer"), "environment.onstopcountdown.noplayer", variables);
			}
			else
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "timeleft",       ev.TimeLeft.ToString()                  },
					{ "ipaddress",      ev.Activator.IPAddress                  },
					{ "name",           ev.Activator.Name                       },
					{ "playerid",       ev.Activator.PlayerID.ToString()        },
					{ "steamid",        ev.Activator.GetParsedUserID()          },
					{ "class",          ev.Activator.PlayerRole.RoleID.ToString()   },
					{ "team",           ev.Activator.PlayerRole.Team.ToString()   }
				};
				this.plugin.SendMessage(Config.GetArray("channels.onstopcountdown.default"), "environment.onstopcountdown.default", variables);
			}
		}
		*/

		/// <summary>
		///  This is the event handler for when the warhead is about to detonate (so before it actually triggers)
		/// </summary>
		/* TODO: [PluginEvent(ServerEventType)]
		public void OnDetonate()
		{
			this.plugin.SendMessage(Config.GetArray("channels.ondetonate"), "environment.ondetonate");
		}
		*/

		/// <summary>
		///  This is the event handler for when the LCZ is decontaminated
		/// </summary>
		/* TODO: [PluginEvent(ServerEventType)]
		public void OnDecontaminate()
		{
			this.plugin.SendMessage(Config.GetArray("channels.ondecontaminate"), "environment.ondecontaminate");
		}
		*/

		/// <summary>
		/// Called when a van/chopper is summoned.
		/// </summary>
		/* TODO: [PluginEvent(ServerEventType)]
		public void OnSummonVehicle(SummonVehicleEvent ev)
		{
			if (ev.IsCI)
			{
				this.plugin.SendMessage(Config.GetArray("channels.onsummonvehicle.chaos"), "environment.onsummonvehicle.chaos");
			}
			else
			{
				this.plugin.SendMessage(Config.GetArray("channels.onsummonvehicle.mtf"), "environment.onsummonvehicle.mtf");
			}
		}
		*/

		/* TODO: [PluginEvent(ServerEventType)]
		public void OnGeneratorFinish(GeneratorFinishEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "room",           ev.Generator.Room.RoomType.ToString()            },
				{ "ipaddress",      ev.ActivatingPlayer.IPAddress                    },
				{ "name",           ev.ActivatingPlayer.Name                         },
				{ "playerid",       ev.ActivatingPlayer.PlayerID.ToString()          },
				{ "steamid",        ev.ActivatingPlayer.GetParsedUserID()            },
				{ "class",          ev.ActivatingPlayer.PlayerRole.RoleID.ToString() },
				{ "team",           ev.ActivatingPlayer.PlayerRole.Team.ToString()   }
			};
			this.plugin.SendMessage(Config.GetArray("channels.ongeneratorfinish"), "environment.ongeneratorfinish", variables);
		}
		*/

		/* TODO: [PluginEvent(ServerEventType)]
		public void OnScpDeathAnnouncement(ScpDeathAnnouncementEvent ev)
		{
			if (!ev.ShouldPlay) return;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "ipaddress",      ev.DeadPlayer.IPAddress                    },
				{ "name",           ev.DeadPlayer.Name                         },
				{ "playerid",       ev.DeadPlayer.PlayerID.ToString()          },
				{ "steamid",        ev.DeadPlayer.GetParsedUserID()            },
				{ "class",          ev.DeadPlayer.PlayerRole.RoleID.ToString() },
				{ "team",           ev.DeadPlayer.PlayerRole.Team.ToString()   }
			};
			this.plugin.SendMessage(Config.GetArray("channels.onscpdeathannouncement"), "environment.onscpdeathannouncement", variables);
		}
		*/

		/* TODO: [PluginEvent(ServerEventType)]
		public void OnCassieCustomAnnouncement(CassieCustomAnnouncementEvent ev)
		{
			if (!ev.Allow) return;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "words", ev.Words }
			};

			this.plugin.SendMessage(Config.GetArray("channels.oncassiecustomannouncement"), "environment.oncassiecustomannouncement", variables);
		}
		*/

		/* TODO: [PluginEvent(ServerEventType)]
		public void OnCassieTeamAnnouncement(CassieTeamAnnouncementEvent ev)
		{
			if (!ev.Allow) return;

			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "cassieunitname", ev.CassieUnitName },
				{ "scpsleft", ev.SCPsLeft.ToString()  }
			};

			this.plugin.SendMessage(Config.GetArray("channels.oncassieteamannouncement"), "environment.oncassieteamannouncement", variables);
		}
		*/
	}
}