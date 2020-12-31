using System;
using System.Net;
using System.Threading.Tasks;

namespace BillDeathIndex
{
	/// <summary>
	/// A callback for when bills are downloaded.
	/// </summary>
	public delegate void BillDownloadCallback(Bill[] bills);

	/// <summary>
	/// An interface representing the API downloaders.
	/// </summary>
	public interface IDownloader
	{
		/// <summary>
		/// Downloads the bills for this API.
		/// </summary>
		/// <param name="callback">The bill download callback.</param>
		Task DownloadBills(IDownloaderSettings settings, BillDownloadCallback callback);

		/// <summary>
		/// Makes the request to the API, downloads the bills, and returns them to the ProcessBillResponses method.
		/// </summary>
		/// <returns>A Bill response object.</returns>
		/// <param name="url">The URL to make the request to.</param>
		/// <param name="year">The current year being requested, if known.</param>
		IBillResponse RequestBills(string url, int year);

		/// <summary>
		/// Makes the request to the API asynchronously, downloads the bills, and returns them to the ProcessBillResponses method.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		void RequestBillsAsync(string url, BillDownloadCallback OnBillsDownloaded);

		/// <summary>
		/// Processes the response from the RequestBillsAsync method.
		/// </summary>
		/// <param name="request">The original web request.</param>
		/// <param name="requestResponse">The bill request response.</param>
		void ProcessBillResponse(HttpWebRequest request, IAsyncResult requestResponse, BillDownloadCallback OnBillsDownloaded);
	}
}
