using System;
using System.IO;
using System.Threading.Tasks;

using System.Linq;

using BillDeathIndex.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BillDeathIndex.States.NY
{
	public class NYSRunner : IRunner
	{
		/// <summary>
		/// The path to the results file.
		/// </summary>
		public static string RESULTS_PATH
		{
			get
			{
				return Path.GetFullPath("files/states/ny-expanded.json");
			}
		}

		/// <summary>
		/// Gets a value indicating whether this runner <see cref="T:BillDeathIndex.States.NY.NYSRunner"/> has met its end condition.
		/// </summary>
		/// <value><c>true</c> if end condition met; otherwise, <c>false</c>.</value>
		public bool EndConditionMet { get; private set; }

		/// <summary>
		/// The NYS API object for this runner.
		/// </summary>
		private NYSAPI api;

		/// <summary>
		/// A StreamWriter that can handle JSON being written from multiple threads.
		/// </summary>
		private JSONThreadWriter writer;

		/// <summary>
		/// The FileStream for the JSONThreadWriter.
		/// </summary>
		private FileStream fs;

		public NYSRunner()
		{
			EndConditionMet = false;
		}

		/// <summary>
		/// Run the NYS API downloading and evaluating.
		/// </summary>
		public void Run()
		{
			int menuSelect = Logger.GetMenuSelection(new string[]{
				"Download for all years",
				"Download for a specific year/years"
			});

			//Makes sure the results CSV file exists.
			IOUtils.EnsureFileExists(RESULTS_PATH);

			//Empties any results from before.
			IOUtils.OverwriteFile(RESULTS_PATH, "{");

			//Open the file stream writer
			fs = IOUtils.WaitForFile(RESULTS_PATH, FileMode.Append, FileAccess.Write, FileShare.Write);
			writer = new JSONThreadWriter(fs);

			//Creates the API object.
			api = new NYSAPI(
				Secrets.GetCredential("State_NY").creds["apiKey"]
			);

			//Creates the downloading task
			Task.Run(() => api.DownloadBills(new NYSAPISettings(menuSelect == 0), HandleDownloadedBills, HandleDownloadsFinished));
		}

		/// <summary>
		/// Handles bills that are downloaded.
		/// </summary>
		/// <param name="bills">The downloaded bills.</param>
		public void HandleDownloadedBills(Bill[] bills)
		{
			try
			{
				//The dictionary to serialize and add to the file.
				Dictionary<string, SavableBill> newBills = new Dictionary<string, SavableBill>();

				foreach (NYSBill bill in (NYSBill[])bills)
				{
					//Set the bill death level
					bill.deathLevel = DeathEvaluator.EvaluateIndicators(GetIndicators(bill));

					//Skip living bills.
					if (bill.deathLevel == DeathEvaluator.BillDeathLevel.Alive)
						continue;

					//Create a small amount of information to save about a bill.
					SavableBill savableBill = new SavableBill(
						bill.printNo,
						bill.session,
						bill.title,
						bill.summary,
						bill.amendments.items[""].fullText,
						bill.signed,
						bill.adopted,
						bill.vetoed,
						bill.deathLevel
					);

					//Add the new bill.
					newBills.Add(bill.printNo, savableBill);
				}

				string JSONlines = JsonConvert.SerializeObject(newBills);
				JSONlines = JSONlines.Substring(1, JSONlines.Length - 2);

				if (JSONlines.Length > 0)
				{
					//Write the info
					writer.WriteLine(JSONlines);
				}

				//Track the end condition.
				api.billsProcessed += bills.Length;
			}
			catch (Exception e)
			{
				Logger.LogError(e);
			}
		}

		/// <summary>
		/// Handles when the bill downloads are finished.
		/// </summary>
		public void HandleDownloadsFinished()
		{
			//Write the final content.
			writer.WriteLine("}", includeComma: false);

			//Cleanup
			writer.Close();
			writer.Dispose();
			fs.Close();
			fs.Dispose();

			//Set the end condition.
			EndConditionMet = true;
		}

		/// <summary>
		/// Gets the death indicators for a bill.
		/// </summary>
		/// <returns>The indicators.</returns>
		/// <param name="bill">The bill.</param>
		public DeathEvaluator.BillDeathIndicators GetIndicators(Bill bill)
		{
			NYSBill nysBill = (NYSBill)bill;

			//Start the indicators
			DeathEvaluator.BillDeathIndicators indicators = DeathEvaluator.BillDeathIndicators.None;

			//Get the most recent action for this bill.
			NYSBill.NYSBillActions.NYSBillAction latestAction = nysBill.actions.items.OrderByDescending(arg => arg.sequenceNo).First();
			NYSBill.NYSBillActions.NYSBillAction secondLatestAction = nysBill.actions.items.FirstOrDefault(arg => arg.sequenceNo == latestAction.sequenceNo - 1);

			//Inactive for four months.
			if (latestAction.date.AddMonths(6) < DateTime.Now)
				indicators |= DeathEvaluator.BillDeathIndicators.Inactivity_SixMonths;


			//Inactive for four months.
			if (latestAction.date.AddYears(1) < DateTime.Now)
				indicators |= DeathEvaluator.BillDeathIndicators.Inactivity_OneYear;
			
			//If the most action was it being referred, say so.
			if (latestAction.text.StartsWith("referred", StringComparison.CurrentCultureIgnoreCase))
				indicators |= DeathEvaluator.BillDeathIndicators.Referred_Committee;

			//If the action before the latest was referral to a committee, than this action must be occurring after committee approval.
			if (secondLatestAction.sequenceNo != 0 && //Ensures the action actually occurred
				secondLatestAction.text.StartsWith("referred", StringComparison.CurrentCultureIgnoreCase))
				indicators |= DeathEvaluator.BillDeathIndicators.Approval_Committee;

			//If the latest action was it being passed, or if the second most recent action had it either passed or delivered after a passing, then it was recently approved by a chamber.
			if (latestAction.text.StartsWith("passed", StringComparison.CurrentCultureIgnoreCase) ||
			   (
				    secondLatestAction.sequenceNo != 0 && //Ensures the action actually occurred
				    (secondLatestAction.text.StartsWith("passed", StringComparison.CurrentCultureIgnoreCase) ||
				     secondLatestAction.text.StartsWith("delivered", StringComparison.CurrentCultureIgnoreCase))
			   ))
				indicators |= DeathEvaluator.BillDeathIndicators.Approval_Chamber;

			//If the latest action was the bill being delivered to the governor and it happened more than 30 days prior, it's considered pocket vetoed.
			if (latestAction.text.Equals("delivered to governor", StringComparison.CurrentCultureIgnoreCase) && latestAction.date.AddDays(30) < DateTime.Now)
				indicators |= DeathEvaluator.BillDeathIndicators.Vetoed_Pocket;

			//If the bill was vetoed
			if (nysBill.vetoed)
				indicators |= DeathEvaluator.BillDeathIndicators.Vetoed;

			//Get the number of non-incumbent (co)sponsors.
			NYSRepresentative[] cosponsors = nysBill.amendments.items.First().Value.coSponsors.items;
			int nonIncumbentCosponsors = cosponsors.Count(arg => !arg.incumbent);
			nonIncumbentCosponsors += (nysBill.sponsor != null && nysBill.sponsor.member != null && !nysBill.sponsor.member.incumbent ? 1 : 0);

			//If half or more of the (co)sponsors are no longer in office, the majority flag is set.
			if (nonIncumbentCosponsors >= (cosponsors.Length + 1) / 2)
				indicators |= DeathEvaluator.BillDeathIndicators.Sponsors_Majority;

			//If all (co)sponsors are no longer in office, the all flag is set.
			if (nonIncumbentCosponsors == cosponsors.Length + 1)
				indicators |= DeathEvaluator.BillDeathIndicators.Sponsors_All;

			//Returns the result.
			return indicators;
		}
	}
}
