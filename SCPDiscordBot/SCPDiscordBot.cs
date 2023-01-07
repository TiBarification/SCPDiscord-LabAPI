using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SCPDiscord
{
	public class SCPDiscordBot
	{
		public static string[] commandlineArguments;

		public static void Main(string[] args)
		{
			commandlineArguments = args;
			new SCPDiscordBot().MainAsync().GetAwaiter().GetResult();
		}

		private async Task MainAsync()
		{
			Logger.Log("Starting SCPDiscord version " + GetVersion() + "...", LogID.GENERAL);
			try
			{
				try
				{
					ConfigParser.LoadConfig();
				}
				catch (Exception e)
				{
					Logger.Fatal("Error loading config!\n" + e);
					return;
				}

				await DiscordAPI.Init();

				new Thread(() => new StartNetworkSystem()).Start();
				new Thread(() => new StartMessageScheduler()).Start();

				// Block this task until the program is closed.
				await Task.Delay(-1);
			}
			catch (Exception e)
			{
				Logger.Fatal("Fatal error:", LogID.GENERAL);
				Logger.Fatal(e.ToString(), LogID.GENERAL);
				Console.ReadLine();
			}
		}

		public static string GetVersion()
		{
			Version version = Assembly.GetEntryAssembly()?.GetName().Version;
			return version?.Major + "." + version?.Minor + "." + version?.Build + (version?.Revision == 0 ? "" : "-" + (char)(64 + version?.Revision ?? 0));
		}
	}
}
