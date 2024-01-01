using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPDiscord
{
    public static class Utilities
	{
        public static string SecondsToCompoundTime(long seconds)
        {
            if (seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            if (seconds == 0) return "0 sec";

            TimeSpan span = TimeSpan.FromSeconds(seconds);
            int[] parts = { span.Days / 365, span.Days % 365 / 31,  span.Days % 365 % 31, span.Hours, span.Minutes, span.Seconds };
            string[] units = { " year", " month", " day", " hour", " minute", " second" };

            return string.Join(", ",
                from index in Enumerable.Range(0, units.Length)
                where parts[index] > 0
                select parts[index] + (parts[index] == 1 ? units[index] : units[index] + "s"));
        }

		public static string TicksToCompoundTime(long ticks)
		{
			return SecondsToCompoundTime(ticks / TimeSpan.TicksPerSecond);
		}

		public static string GetParsedUserID(string userID)
		{
			if (!string.IsNullOrWhiteSpace(userID))
			{
				int charLocation = userID.LastIndexOf('@');

				if (charLocation > 0)
				{
					return userID.Substring(0, charLocation);
				}
			}
			return null;
		}

		public static bool IsPossibleSteamID(string steamID)
		{
			return steamID.Length >= 17 && ulong.TryParse(steamID.Replace("@steam", ""), out ulong _);
		}

		public static string EscapeDiscordFormatting(string input)
		{
			input = input.Replace("`", "\\`");
			input = input.Replace("*", "\\*");
			input = input.Replace("_", "\\_");
			input = input.Replace("~", "\\~");
			return input;
		}

		public static LinkedList<string> ParseListIntoMessages(List<string> listItems)
		{
			LinkedList<string> messages = new LinkedList<string>();
			foreach (string listItem in listItems)
			{
				if (messages.Last?.Value?.Length + listItem?.Length < 2048)
				{
					messages.Last.Value += listItem + "\n";
				}
				else
				{
					messages.AddLast(listItem?.Trim());
				}
			}

			return messages;
		}
	}
}
