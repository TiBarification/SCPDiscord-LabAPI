using System.Collections.Generic;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using Respawning;

namespace SCPDiscord.EventListeners
{
	class TeamEventListener
	{
		private readonly SCPDiscord plugin;

		public TeamEventListener(SCPDiscord plugin)
		{
			this.plugin = plugin;
		}

		[PluginEvent(ServerEventType.TeamRespawn)]
		public void OnTeamRespawn(SpawnableTeamType team)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				//{ "players",    ev.PlayerList.ToString()    }
			};

			if (team == SpawnableTeamType.ChaosInsurgency)
			{
				plugin.SendMessage(Config.GetArray("channels.onteamrespawn.ci"), "team.onteamrespawn.ci", variables);
			}
			else
			{
				plugin.SendMessage(Config.GetArray("channels.onteamrespawn.mtf"), "team.onteamrespawn.mtf", variables);
			}

		}

		/*
		[PluginEvent(ServerEventType.RoundStart)]
		public void OnSetMTFUnitName(SetMTFUnitNameEvent ev)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>
			{
				{ "name", ev.Unit }
			};
			plugin.SendMessage(Config.GetArray("channels.onsetntfunitname"), "team.onsetntfunitname", variables);
		}
		*/
	}
}
