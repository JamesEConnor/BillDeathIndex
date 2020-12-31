using System;

using BillDeathIndex.States.NY;

namespace BillDeathIndex
{
	class MainClass
	{
		static string[] MENU_OPTIONS = new string[] {
			"New York State API download",
			"New York State CSV Conversion",
			"Congress API download",
			"Congress CSV Conversion"
		};

		public static void Main(string[] args)
		{
			//Print the welcome and handle menu selection.
			PrintWelcome();
			int menuSelect = GetMenuSelection();

			//Clear the console once an option is selected.
			Console.Clear();

			//NYS API option
			if (menuSelect == 0)
			{
				//Run NYS API.
				NYSRunner runner = new NYSRunner();
				runner.Run();

				while (!runner.EndConditionMet) { }
			}
		}

		/// <summary>
		/// Prints the welcome info.
		/// </summary>
		public static void PrintWelcome()
		{
			string welcomeBanner = System.IO.File.ReadAllText(System.IO.Path.GetFullPath("files/ascii-banner.txt"));

			Console.WriteLine(welcomeBanner);
			Console.WriteLine("This tool is for downloading information about bills from various legislatures and analyzing them to evaluate which bills are considered 'dead'.");
		}

		/// <summary>
		/// Gets the selection from the menu.
		/// </summary>
		/// <returns>The menu selection as an integer.</returns>
		public static int GetMenuSelection()
		{
			//Instructions
			Console.WriteLine("(Use the arrow keys to navigate. Press enter to select.)");

			//Print out the options and set the cursor to the first line.
			for (int a = 0; a < MENU_OPTIONS.Length; a++)
				Console.WriteLine(string.Format("{0}. {1}", a + 1, MENU_OPTIONS[a]));

			//Set the cursor position to the first option
			Console.SetCursorPosition(0, Console.CursorTop - MENU_OPTIONS.Length);

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
					if (currentSelection + 1 < MENU_OPTIONS.Length)
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
			Console.SetCursorPosition(0, Console.CursorTop + (MENU_OPTIONS.Length - currentSelection) + 1);

			//Return the result.
			return currentSelection;
		}
	}
}
