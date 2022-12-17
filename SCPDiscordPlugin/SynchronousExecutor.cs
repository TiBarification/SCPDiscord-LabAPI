using System.Collections.Concurrent;
using System.Collections.Generic;
using PluginAPI.Core;
using SCPDiscord.Interface;
using UnityEngine;

namespace SCPDiscord
{
	public class SynchronousExecutor : MonoBehaviour
	{
		private readonly ConcurrentQueue<ConsoleCommand> queuedCommands = new ConcurrentQueue<ConsoleCommand>();
		private readonly ConcurrentQueue<string> queuedRoleSyncCommands = new ConcurrentQueue<string>();

		public void ScheduleDiscordCommand(ConsoleCommand command)
		{
			queuedCommands.Enqueue(command);
		}

		public void ScheduleRoleSyncCommand(string command)
        {
			SCPDiscord.plugin.Error("Length: " + queuedRoleSyncCommands?.Count);
			queuedRoleSyncCommands.Enqueue(command);
        }

		public void FixedUpdate()
		{
			while(queuedCommands.TryDequeue(out ConsoleCommand command))
			{
				string response = Server.RunCommand(command.Command);
				Dictionary<string, string> variables = new Dictionary<string, string>
				{
					{ "feedback", response }
				};

				EmbedMessage embed = new EmbedMessage
				{
					Colour = EmbedMessage.Types.DiscordColour.Orange,
					ChannelID = command.ChannelID
				};

				SCPDiscord.plugin.SendEmbedWithMessageByID(embed, "botresponses.consolecommandfeedback", variables);
			}

			while(queuedRoleSyncCommands.TryDequeue(out string stringCommand))
			{
				SCPDiscord.plugin.Debug("RoleSync command response: " + Server.RunCommand(stringCommand));
			}
		}
	}
}