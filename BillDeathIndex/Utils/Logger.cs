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
		/// Writes a message of messageType with the specified color.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="messageType">Message type.</param>
		/// <param name="color">Color.</param>
		private static void Log(string message, string messageType, ConsoleColor color)
		{
			//Makes the message type exactly five characters.
			messageType = (messageType.Length > 5 ? messageType.Substring(0, 5) : messageType);
			messageType = messageType.ToUpper().PadLeft(5, ' ');

			//Gets the properly formatted string
			string result = string.Format("[{0}][{1}][{2}]\t{3}", DateTime.Now, messageType, logCount, message);

			//Prints the correctly colored result.
			Console.ForegroundColor = color;
			Console.WriteLine(result);
			Console.ResetColor();

			logCount++;
		}

		/// <summary>
		/// Writes a message of messageType with the specified color.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="messageType">Message type.</param>
		/// <param name="color">Color.</param>
		public static void Log(object message, string messageType, ConsoleColor color)
		{
			Log(message.ToString(), messageType, color);
		}

		/// <summary>
		/// Logs a message in the console window with the specified color.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="color">The color to log the message with.</param>
		public static void Log(string message, ConsoleColor color)
		{
			Log(message, "info", color);
		}

		/// <summary>
		/// Logs a message in the console window with the specified color.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="color">The color to log the message with.</param>
		public static void Log(object message, ConsoleColor color)
		{
			Log(message, "info", color);
		}

		/// <summary>
		/// Logs a message in the console window.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Log(string message)
		{
			Log(message, "info", Console.ForegroundColor);
		}

		/// <summary>
		/// Logs a message in the console window.
		/// </summary>
		/// <param name="message">Message.</param>
		public static void Log(object message)
		{
			Log(message, "info", Console.ForegroundColor);
		}

		/// <summary>
		/// Logs a warning message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogWarning(string message)
		{
			Log(message, "warn", ConsoleColor.Yellow);
		}

		/// <summary>
		/// Logs a warning message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogWarning(object message)
		{
			Log(message, "warn", ConsoleColor.Yellow);
		}

		/// <summary>
		/// Logs an error message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogError(string message)
		{
			Log(message, "error", ConsoleColor.Red);
		}

		/// <summary>
		/// Logs an error message in the console window.
		/// </summary>
		/// <param name="message">The message to log.</param>
		public static void LogError(object message)
		{
			Log(message, "error", ConsoleColor.Red);
		}

		/// <summary>
		/// Logs a debug message in the console window.
		/// </summary>
		/// <param name="message">The message to debug.</param>
		public static void Debug(string message)
		{
			Log(message, "debug", ConsoleColor.Blue);
		}

		/// <summary>
		/// Logs a debug message in the console window.
		/// </summary>
		/// <param name="message">The message to debug.</param>
		public static void Debug(object message)
		{
			Log(message, "debug", ConsoleColor.Blue);
		}
	}
}
