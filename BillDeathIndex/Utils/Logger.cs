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
			string result = string.Format("[{0}][{1}][{2}]\t{3}", DateTime.Now, messageType, logCount.ToString().PadLeft(4, '0'), message);

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

		/// <summary>
		/// Gets input from a user.
		/// </summary>
		/// <returns>The inputted string.</returns>
		public static string GetInput()
		{
			//Gets the properly formatted string
			string prompt = string.Format("[{0}][INPUT][{1}]\t", DateTime.Now, logCount.ToString().PadLeft(4, '0'));

			//Prints the prompt and waits for input.
			Console.ForegroundColor = ConsoleColor.Magenta;

			Console.Write(prompt);
			string result = Console.ReadLine();

			Console.ResetColor();

			logCount++;

			return result;
		}

		/// <summary>
		/// Gets the selection from the menu.
		/// </summary>
		/// <returns>The menu selection as an integer.</returns>
		public static int GetMenuSelection(string[] options)
		{
			//Instructions
			Console.WriteLine("(Use the arrow keys to navigate. Press enter to select.)");

			//Print out the options and set the cursor to the first line.
			for (int a = 0; a < options.Length; a++)
				Console.WriteLine(string.Format("{0}. {1}", a + 1, options[a]));

			//Set the cursor position to the first option
			Console.SetCursorPosition(0, Console.CursorTop - options.Length);

			//The currently selected option
			int currentSelection = 0;

			//Handles user input
			while (true)
			{
				//Get the currently pressed key
				ConsoleKey pressedKey = Console.ReadKey().Key;

				//If the user pressed the up arrow, navigate up the menu
				if (pressedKey == ConsoleKey.UpArrow)
				{
					//Ensure the number remains in place
					Console.Write("\r" + (currentSelection + 1));

					//If it won't take the cursor out of bounds
					if (currentSelection - 1 >= 0)
					{
						//Set the selection and position properly
						currentSelection--;

						Console.SetCursorPosition(0, Console.CursorTop - 1);
					}
					else
						Console.SetCursorPosition(0, Console.CursorTop);
				}
				//If the user pressed the down arrow, navigate down the menu
				else if (pressedKey == ConsoleKey.DownArrow)
				{
					//Ensure the number remains in place
					Console.Write("\r" + (currentSelection + 1));

					//If it won't take the cursor out of bounds
					if (currentSelection + 1 < options.Length)
					{
						//Set the selection and position properly
						currentSelection++;

						Console.SetCursorPosition(0, Console.CursorTop + 1);
					}
					else
						Console.SetCursorPosition(0, Console.CursorTop);
				}
				//If the user presses enter, they've selected that one.
				else if (pressedKey == ConsoleKey.Enter)
					break;
			}

			//Set the cursor position to the line after the last option.
			Console.SetCursorPosition(0, Console.CursorTop + (options.Length - currentSelection) + 1);

			//Return the result.
			return currentSelection;
		}
	}
}
