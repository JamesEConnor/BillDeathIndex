using System;

namespace BillDeathIndex.Utils
{
	public class Logger
	{
		/// <summary>
		/// The log count.
		/// </summary>
		static int logCount = 0;

		/// <summary>
		/// Logs a message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void Log(string message)
		{
			string result = "";
			result += DateTime.Now + "\t";
			result += logCount + ":";
			result += message;

			Console.WriteLine(result);
		}

		/// <summary>
		/// Logs a warning message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogWarning(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Log(message);
			Console.ResetColor();
		}

		/// <summary>
		/// Logs an error message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Log(message);
			Console.ResetColor();
		}
	}
}
