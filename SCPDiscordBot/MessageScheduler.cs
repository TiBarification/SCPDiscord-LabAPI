using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

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

	public static async void Init()
	{
		while (true)
		{
			Thread.Sleep(1000);

			// If we havent connected to discord yet wait until we do
			if (!DiscordAPI.instance?.connected ?? false) continue;

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
							Logger.Warn("Tried to send too much at once, waiting one second to send the rest.");
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
}