using System;
using System.Collections.Generic;
using BillDeathIndex.States.NY;
using BillDeathIndex.Utils;
using Newtonsoft.Json;

namespace BillDeathIndex
{
	class MainClass
	{
		static string[] MENU_OPTIONS = new string[] {
			"New York State API download",
			"New York State CSV Conversion"
		};

		public static void Main(string[] args)
		{
			//Print the welcome and handle menu selection.
			PrintWelcome();
			int menuSelect = Logger.GetMenuSelection(MENU_OPTIONS);

			//Clear the console once an option is selected.
			Console.Clear();

			//NYS API option
			if (menuSelect == 0)
			{
				//Run NYS API.
				NYSRunner runner = new NYSRunner();
				runner.Run();

				while (!runner.EndConditionMet) { }

				Logger.Log("Completed downloads", "END", ConsoleColor.Green);
			}
			else if (menuSelect == 1)
			{
				Logger.Log("Checking for file.");

				if (!System.IO.File.Exists(NYSRunner.RESULTS_PATH))
				{
					Logger.LogError("Bills have not been downloaded yet. Download script must be run before attempting CSV conversion.");
					return;
				}

				Logger.Log("File found.");

				//Create a JSON Stream Reader
				using (var reader = new JSONArrayStreamReader(NYSRunner.RESULTS_PATH))
				{
					//Create a converter
					NYSConverter converter = new NYSConverter();
					converter.SaveHeaderLine();

					//Counts the number of bills.
					int counter = 0;

					//Loops through each bill one at a time.
					foreach (string JSON in reader)
					{
						//Print progress
						//if (counter % 1000 == 0)
							Logger.Log("Saved " + counter + " bills");

						//Converts the bill
						NYSBill nysBill = JsonConvert.DeserializeObject<NYSBill>(JSON);

						//Save a converted CSV of the bill.
						converter.SaveConversion(nysBill);

						//Track the number of bills saved.
						counter++;
					}
				}

				Logger.Log("Completed conversion", "END", ConsoleColor.Green);
			}

			//Wrap up and allow the user to read through the logs.
			Console.WriteLine("\nPress any key to exit...");
			Console.ReadKey();
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
	}
}
