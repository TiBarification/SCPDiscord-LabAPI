using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace SCPDiscord;

// Separate class to run the thread
public class StartMessageScheduler
{
	public StartMessageScheduler()
	{
		MessageScheduler.Init();
	}
}

public static class MessageScheduler
{
	private static ConcurrentDictionary<ulong, ConcurrentQueue<string>> messageQueues = new ConcurrentDictionary<ulong, ConcurrentQueue<string>>();
	private static List<InteractionContext> interactionCache = new List<InteractionContext>();

	public static async void Init()
	{
		while (true)
		{
			Thread.Sleep(1000);

			// If we havent connected to discord yet wait until we do
			if (!DiscordAPI.instance?.connected ?? false) continue;

			// Clean old interactions from cache
			interactionCache.RemoveAll(x => x.InteractionId.GetSnowflakeTime() < DateTimeOffset.Now - TimeSpan.FromSeconds(30));

			try
			{
				foreach (KeyValuePair<ulong, ConcurrentQueue<string>> channelQueue in messageQueues)
				{
					string finalMessage = "";
					while(channelQueue.Value.TryPeek(out string nextMessage))
					{
						// If message is too long, abort and send the rest next time
						if (finalMessage.Length + nextMessage.Length >= 2000)
						{
							Logger.Warn("Tried to send too much at once (Current: " + finalMessage.Length + " Next: " +  nextMessage.Length + "), waiting one second to send the rest.", LogID.DISCORD);
							break;
						}

						// This if shouldn't be needed but might as well just in case some multi-threading shenanigans happen
						if (channelQueue.Value.TryDequeue(out nextMessage))
						{
							finalMessage += nextMessage;
							finalMessage += "\n";
						}
					}

					if (string.IsNullOrWhiteSpace(finalMessage)) continue;

					if (finalMessage.EndsWith("\n"))
					{
						finalMessage = finalMessage.Remove(finalMessage.Length - 1);
					}

					await DiscordAPI.SendMessage(channelQueue.Key, finalMessage);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}

	public static void QueueMessage(ulong channelID, string message)
	{
		ConcurrentQueue<string> channelQueue = messageQueues.GetOrAdd(channelID, new ConcurrentQueue<string>());
		channelQueue.Enqueue(message);
	}

	public static bool TryUncacheInteraction(ulong interactionID, out InteractionContext interaction)
	{
		interaction = interactionCache.FirstOrDefault(x => x.InteractionId == interactionID);
		return interactionCache.Remove(interaction);
	}

	public static void CacheInteraction(InteractionContext interaction)
	{
		interactionCache.Add(interaction);
	}
}