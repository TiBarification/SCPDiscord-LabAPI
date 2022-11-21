using System.Collections.Concurrent;
using System.Collections.Generic;
using PluginAPI.Core;
using SCPDiscord.Interface;

namespace SCPDiscord
{
	public class SynchronousExecutor
	{
		private readonly SCPDiscord plugin;
		private readonly ConcurrentQueue<ConsoleCommand> queuedCommands = new ConcurrentQueue<ConsoleCommand>();
		private readonly ConcurrentQueue<string> queuedRoleSyncCommands = new ConcurrentQueue<string>();
		public SynchronousExecutor(SCPDiscord pl)
		{
			plugin = pl;
		}

		public void ScheduleDiscordCommand(ConsoleCommand command)
		{
			queuedCommands.Enqueue(command);
		}

		public void ScheduleRoleSyncCommand(string command)
        {
			queuedRoleSyncCommands.Enqueue(command);
        }

		// TODO: Find new way to get unity to call this function
		public void OnFixedUpdate(FixedUpdateEvent ev)
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

				plugin.SendEmbedWithMessageByID(embed, "botresponses.consolecommandfeedback", variables);
			}

			while(queuedRoleSyncCommands.TryDequeue(out string stringCommand))
			{
				plugin.Debug("RoleSync command response: " + Server.RunCommand(stringCommand));
			}
		}
	}
}