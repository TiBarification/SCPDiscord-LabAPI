using System.Collections.Generic;
using LiteNetLib;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace SCPDiscord.EventListeners
{
	internal class RoundEventListener
	{
		private readonly SCPDiscord plugin;

		public RoundEventListener(SCPDiscord plugin)
		{
			this.plugin = plugin;
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void OnRoundStart()
		{
			plugin.SendMessage(Config.GetArray("channels.onroundstart"), "round.onroundstart");
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
			plugin.SendMessage(Config.GetArray("channels.onconnect"), "round.onconnect", variables);
		}

		/*
		[PluginEvent(ServerEventType.RoundEnd)]
		public void OnRoundEnd(RoundEndEvent ev)
		{
			if (this.plugin.roundStarted && ev.Round.Duration > 60)
			{
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "duration",          (ev.Round.Duration/60).ToString()            },
					{ "dclassalive",        ev.Round.Stats.DClassAlive.ToString()       },
					{ "dclassdead",         ev.Round.Stats.DClassDead.ToString()        },
					{ "dclassescaped",      ev.Round.Stats.DClassEscaped.ToString()     },
					{ "dclassstart",        ev.Round.Stats.DClassStart.ToString()       },
					{ "mtfalive",           ev.Round.Stats.MTFAlive.ToString()          },
					{ "scientistsalive",    ev.Round.Stats.ScientistsAlive.ToString()   },
					{ "scientistsdead",     ev.Round.Stats.ScientistsDead.ToString()    },
					{ "scientistsescaped",  ev.Round.Stats.ScientistsEscaped.ToString() },
					{ "scientistsstart",    ev.Round.Stats.ScientistsStart.ToString()   },
					{ "scpalive",           ev.Round.Stats.SCPAlive.ToString()          },
					{ "scpdead",            ev.Round.Stats.SCPDead.ToString()           },
					{ "scpkills",           ev.Round.Stats.SCPKills.ToString()          },
					{ "scpstart",           ev.Round.Stats.SCPStart.ToString()          },
					{ "warheaddetonated",   ev.Round.Stats.WarheadDetonated.ToString()  },
					{ "zombies",            ev.Round.Stats.Zombies.ToString()           }
				};
				this.plugin.SendMessage(Config.GetArray("channels.onroundend"), "round.onroundend", variables);
				this.plugin.roundStarted = false;
			}
		}
		*/


		[PluginEvent(ServerEventType.WaitingForPlayers)]
		public void OnWaitingForPlayers()
		{
			plugin.SendMessage(Config.GetArray("channels.onwaitingforplayers"), "round.onwaitingforplayers");
		}

		[PluginEvent(ServerEventType.RoundRestart)]
		public void OnRoundRestart()
		{
			plugin.SendMessage(Config.GetArray("channels.onroundrestart"), "round.onroundrestart");
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
			this.plugin.SendMessage(Config.GetArray("channels.onsetservername"), "round.onsetservername", variables);
		}
		*/
	}
}