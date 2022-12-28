using System;
using System.Collections.Generic;
using System.Globalization;
using LiteNetLib;
using PluginAPI.Core;
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
					//{ "warheaddetonated",   Statistics.CurrentRound.WarheadDetonated.ToString()  },
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