using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

using BillDeathIndex.Utils;
using System.Collections.Generic;

namespace BillDeathIndex.States.NY
{
	public class NYSAPI : IDownloader
	{
		/// <summary>
		/// The API key.
		/// </summary>
		readonly string apiKey;

		/// <summary>
		/// Tracks the number of threads.
		/// </summary>
		int threadCount = 0;

		/// <summary>
		/// The maximum number of threads allowed for the downloader.
		/// </summary>
		public const int MAX_THREAD_COUNT = 5;


		//The total number of bills that are to be downloaded across all years.
		public int totalBillsToDownload = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:BillDeathIndex.States.NY.NYSAPI"/> class.
		/// </summary>
		/// <param name="_apiKey">The NYS OpenLegislation API key.</param>
		public NYSAPI(string _apiKey)
		{
			apiKey = _apiKey;
		}

		/// <summary>
		/// Downloads the bills based on settings, then sends them as an array to the callback function.
		/// </summary>
		/// <param name="settings">The download settings.</param>
		/// <param name="OnBillsDownloaded">The callback to send the results to.</param>
		public async Task DownloadBills(IDownloaderSettings settings, BillDownloadCallback OnBillsDownloaded, DownloadsFinished OnBillDownloadFinished)
		{
			try
			{
				//Loop through all the years
				int[] years = ((NYSAPISettings)settings).years;
				for (int y = 0; y < years.Length; y++)
				{
					Logger.Log("Starting year " + years[y]);

					//The base URL to build subsequent URLs off of.
					string baseURL = "https://legislation.nysenate.gov/api/3/bills/" + years[y] + "?key=" + apiKey + "&full=true&fullTextFormat=PLAIN";

					//The initial bill response that will contain information on how many bills exist.
					NYSBillResponse initResponse = (NYSBillResponse)RequestBills(baseURL + "&limit=1", years[y]);
					totalBillsToDownload += initResponse.total;

					Logger.Log("Total bill count raised to " + totalBillsToDownload);

					//Callback for the single downloaded bill.
					OnBillsDownloaded(initResponse.result.items);

					//Download the remaining bills asynchronously
					for (int a = 1; a < initResponse.total; a += 1000)
					{
						Logger.Log(string.Format("Downloading bills {0}-{1} of {2} for session years {3} through {4}.", a, a + 1000, initResponse.total, years[y], years[y] + 1));

						//Ensure the maximum thread count is never surpassed.
						while (threadCount >= MAX_THREAD_COUNT)
							await Task.Delay(1000);

						if (a + 1000 > initResponse.total && y == years.Length - 1)
						{
							//Make the last request synchronously to ensure it finishes before ending the process
							NYSBillResponse finalResponse = (NYSBillResponse)RequestBills(baseURL + "&limit=1000&offset=" + a, years[y]);
							OnBillsDownloaded(finalResponse.result.items);
						}
						else
							RequestBillsAsync(baseURL + "&limit=1000&offset=" + a, OnBillsDownloaded);
					}
				}

				//Callback for when the downloads are finished.
				OnBillDownloadFinished();
			}
			catch (Exception e)
			{
				Logger.LogError(e);
			}
		}

		/// <summary>
		/// Makes the request to the NYS OpenLegislation API, downloads the bills, and returns them to the ProcessBillResponses method.
		/// Used to get the number of bills available.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <param name="year">The current year being requested.</param>
		public IBillResponse RequestBills(string url, int year)
		{
			//Generate the request
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			using (var response = request.GetResponse())
			{
				using (var stream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(stream, System.Text.Encoding.UTF8))
					{
						string responseText = reader.ReadToEnd();

						//Convert to a bill response object and return.
						return JsonConvert.DeserializeObject<NYSBillResponse>(responseText);
					}
				}
			}
		}

		/// <summary>
		/// Makes the request to the NYS OpenLegislation API asynchronously, downloads the bills, and returns them to the ProcessBillResponses method.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		public void RequestBillsAsync(string url, BillDownloadCallback OnBillsDownloaded)
		{
			try
			{
				threadCount++;

				//Generate the request
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

				//Make the request asynchronously
				request.BeginGetResponse(response => ProcessBillResponse(request, response, OnBillsDownloaded), null);
			}
			catch (Exception e)
			{
				Logger.LogError(e);
			}
		}

		/// <summary>
		/// Processes the response from the RequestBillsAsync method.
		/// </summary>
		/// <param name="request">The original web request.</param>
		/// <param name="requestResponse">The bill request response.</param>
		public void ProcessBillResponse(HttpWebRequest request, IAsyncResult requestResponse, BillDownloadCallback OnBillsDownloaded)
		{
			//Create a stream reader from the response to read the text.
			using (var response = request.EndGetResponse(requestResponse))
			{
				using (var stream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(stream))
					{
						//Get the text and convert it to JSON
						string responseText = reader.ReadToEnd();

						try
						{
							//Get the bill response and passes it to the callback.
							NYSBillResponse billResponse = JsonConvert.DeserializeObject<NYSBillResponse>(responseText);
							OnBillsDownloaded(billResponse.result.items);
						}
						catch (Exception e)
						{
							//75513
							Logger.LogError(e);
							//Logger.LogError(string.Join("\n", responseText.Split('\n').Skip(75480).Take(60)));
						}
					}
				}
			}

			threadCount--;
		}
	}

	public struct NYSAPISettings : IDownloaderSettings
	{
		/// <summary>
		/// The lowest year from which results are available.
		/// </summary>
		public const int MIN_YEAR = 2009;

		/// <summary>
		/// The years to download bills from.
		/// </summary>
		public int[] years;

		public NYSAPISettings(bool downloadAllYears)
		{
			//Either download bills from every year, or just from the current year.
			if (downloadAllYears)
			{
				//The number of years between the lowest available year and this year, inclusive.
				int numberOfYears = DateTime.Now.Year - MIN_YEAR + 1;

				//Accounts for situations where bills may have been added for the following year.
				if (DateTime.Now.Year % 2 == 1)
					numberOfYears++;

				//Set the year array
				years = new int[numberOfYears/2];
				for (int a = 0; a < years.Length; a++)
					years[a] = MIN_YEAR + (a * 2);
			}
			else
			{
				years = new int[] { DateTime.Now.Year };
			}
		}
	}
}
