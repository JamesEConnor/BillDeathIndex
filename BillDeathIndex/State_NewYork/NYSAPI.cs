using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

using BillDeathIndex.Utils;
using System.Collections.Generic;

namespace BillDeathIndex.States.NY
{
	public class NYSAPI : IDownloader
	{
		string apiKey;

		private Dictionary<int, List<NYSBill>> resultsByYear = new Dictionary<int, List<NYSBill>>();

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
		/// <param name="callback">The callback to send the results to.</param>
		public void DownloadBills(IDownloaderSettings settings, BillDownloadCallback callback)
		{
			
		}

		/// <summary>
		/// Makes the request to the NYS OpenLegislation API, downloads the bills, and returns them to the ProcessBillResponses method.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <param name="year">The current year being requested.</param>
		public void RequestBills(string url, int year)
		{
			//Generate the request
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

			//Make the request asynchronously
			request.BeginGetResponse(response => ProcessBillResponse(request, response, year), null);
		}

		/// <summary>
		/// Processes the response from the RequestBills method.
		/// </summary>
		/// <param name="request">The original web request.</param>
		/// <param name="requestResponse">The bill request response.</param>
		/// <param name="year">The year the request is from.</param>
		public void ProcessBillResponse(HttpWebRequest request, IAsyncResult requestResponse, int year)
		{
			//Create a stream reader from the response to read the text.
			WebResponse response = request.EndGetResponse(requestResponse);
			StreamReader reader = new StreamReader(response.GetResponseStream());

			//Get the text and convert it to JSON
			string responseText = reader.ReadToEnd();

			NYSBillResponse billResponse = JsonConvert.DeserializeObject<NYSBillResponse>(responseText);

			resultsByYear[year].AddRange(billResponse.result.items);

			string oldURL = request.RequestUri.PathAndQuery;
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

				//Set the year array
				years = new int[numberOfYears];
				for (int a = 0; a < years.Length; a++)
					years[a] = MIN_YEAR + a;
			}
			else
			{
				years = new int[] { DateTime.Now.Year };
			}
		}
	}
}
