using System;
using System.Net;
using System.Threading.Tasks;

namespace BillDeathIndex
{
	/// <summary>
	/// A callback for when bills are downloaded.
	/// </summary>
	public delegate bool BillDownloadCallback(IBill[] bills);

	/// <summary>
	/// An interface representing the API downloaders.
	/// </summary>
	public interface IDownloader
	{
		/// <summary>
		/// Downloads the bills for this API.
		/// </summary>
		/// <param name="callback">The bill download callback.</param>
		void DownloadBills(IDownloaderSettings settings, BillDownloadCallback callback);

		/// <summary>
		/// Makes the request to the API, downloads the bills, and returns them to the ProcessBillResponses method.
		/// </summary>
		/// <param name="url">The URL to make the request to.</param>
		/// <param name="year">The current year being requested, if known.</param>
		void RequestBills(string url, int year);

		/// <summary>
		/// Processes the response from the RequestBills method.
		/// </summary>
		/// <param name="request">The original web request.</param>
		/// <param name="requestResponse">The bill request response.</param>
		/// <param name="year">The year the request is from, if known.</param>
		void ProcessBillResponse(HttpWebRequest request, IAsyncResult requestResponse, int year);
	}
}
