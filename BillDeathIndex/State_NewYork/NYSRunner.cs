using System;
using System.IO;
using System.Threading.Tasks;

using System.Linq;

using BillDeathIndex.Utils;
using Newtonsoft.Json;

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
				return Path.GetFullPath("files/states/ny.json");
			}
		}

		/// <summary>
		/// Gets a value indicating whether this runner <see cref="T:BillDeathIndex.States.NY.NYSRunner"/> has met its end condition.
		/// </summary>
		/// <value><c>true</c> if end condition met; otherwise, <c>false</c>.</value>
		public bool EndConditionMet { get; private set; }

		/// <summary>
		/// Tracks the number of bills processed, to support meeting the end condition.
		/// </summary>
		private int billsProcessed = 0;

		/// <summary>
		/// The NYS API object for this runner.
		/// </summary>
		private NYSAPI api;

		public NYSRunner()
		{
			EndConditionMet = false;
		}

		/// <summary>
		/// Run the NYS API downloading and evaluating.
		/// </summary>
		public void Run()
		{
			//Makes sure the results CSV file exists.
			IOUtils.EnsureFileExists(RESULTS_PATH);

			//Empties any results from before.
			IOUtils.OverwriteFile(RESULTS_PATH, "[");

			//Creates the API object.
			api = new NYSAPI(
				Secrets.GetCredential("State_NY").creds["apiKey"]
			);

			//Creates the downloading task
			Task.Run(() => api.DownloadBills(new NYSAPISettings(true), HandleDownloadedBills, HandleDownloadsFinished));
		}

		/// <summary>
		/// Handles bills that are downloaded.
		/// </summary>
		/// <param name="bills">The downloaded bills.</param>
		public void HandleDownloadedBills(Bill[] bills)
		{
			try
			{
				FileStream fs = IOUtils.WaitForFile(RESULTS_PATH, FileMode.Append, FileAccess.Write, FileShare.Write);
				StreamWriter writer = new StreamWriter(fs);

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
						bill.title,
						bill.summary,
						bill.deathLevel
					);

					//Generate the CSV lines.
					string JSONlines = JsonConvert.SerializeObject(savableBill);

					//Write the info
					writer.WriteLine(JSONlines);
				}

				//Track the end condition.
				billsProcessed += bills.Length;
				if (billsProcessed == api.totalBillsToDownload)
				{
					writer.WriteLine("]");
					EndConditionMet = true;
				}

				//Cleanup
				writer.Close();
				writer.Dispose();
				fs.Close();
				fs.Dispose();
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
			using (var fs = IOUtils.WaitForFile(RESULTS_PATH, FileMode.Append, FileAccess.Write, FileShare.Write))
			{
				using (var writer = new StreamWriter(fs))
				{
					//Write the last character and end the loop.
					writer.WriteLine("]");
					EndConditionMet = true;
				}
			}
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
			if (latestAction.date.AddMonths(4) < DateTime.Now)
				indicators &= DeathEvaluator.BillDeathIndicators.Inactivity_FourMonths;


			//Inactive for four months.
			if (latestAction.date.AddYears(1) < DateTime.Now)
				indicators &= DeathEvaluator.BillDeathIndicators.Inactivity_OneYear;
			
			//If the most action was it being referred, say so.
			if (latestAction.text.StartsWith("referred", StringComparison.CurrentCultureIgnoreCase))
				indicators &= DeathEvaluator.BillDeathIndicators.Referred_Committee;

			//If the action before the latest was referral to a committee, than this action must be occurring after committee approval.
			if (secondLatestAction.sequenceNo != 0 && //Ensures the action actually occurred
				secondLatestAction.text.StartsWith("referred", StringComparison.CurrentCultureIgnoreCase))
				indicators &= DeathEvaluator.BillDeathIndicators.Approval_Committee;

			//If the latest action was it being passed, or if the second most recent action had it either passed or delivered after a passing, then it was recently approved by a chamber.
			if (latestAction.text.StartsWith("passed", StringComparison.CurrentCultureIgnoreCase) ||
			   (
				    secondLatestAction.sequenceNo != 0 && //Ensures the action actually occurred
				    (secondLatestAction.text.StartsWith("passed", StringComparison.CurrentCultureIgnoreCase) ||
				     secondLatestAction.text.StartsWith("delivered", StringComparison.CurrentCultureIgnoreCase))
			   ))
				indicators &= DeathEvaluator.BillDeathIndicators.Approval_Chamber;

			//If the latest action was the bill being delivered to the governor and it happened more than 30 days prior, it's considered pocket vetoed.
			if (latestAction.text.Equals("delivered to governor", StringComparison.CurrentCultureIgnoreCase) && latestAction.date.AddDays(30) < DateTime.Now)
				indicators &= DeathEvaluator.BillDeathIndicators.Vetoed_Pocket;

			//If the bill was vetoed
			if (nysBill.vetoed)
				indicators &= DeathEvaluator.BillDeathIndicators.Vetoed;

			//Get the number of non-incumbent (co)sponsors.
			NYSRepresentative[] cosponsors = nysBill.amendments.items.First().Value.coSponsors.items;
			int nonIncumbentCosponsors = cosponsors.Count(arg => !arg.incumbent);
			nonIncumbentCosponsors += (nysBill.sponsor != null && nysBill.sponsor.member != null && !nysBill.sponsor.member.incumbent ? 1 : 0);

			//If half or more of the (co)sponsors are no longer in office, the majority flag is set.
			if (nonIncumbentCosponsors >= (cosponsors.Length + 1) / 2)
				indicators &= DeathEvaluator.BillDeathIndicators.Sponsors_Majority;

			//If all (co)sponsors are no longer in office, the all flag is set.
			if (nonIncumbentCosponsors == cosponsors.Length + 1)
				indicators &= DeathEvaluator.BillDeathIndicators.Sponsors_All;

			//Returns the result.
			return indicators;
		}
	}
}
