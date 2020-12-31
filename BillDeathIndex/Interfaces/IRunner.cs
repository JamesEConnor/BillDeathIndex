using System;

namespace BillDeathIndex
{
	/// <summary>
	/// A delegate for calling when bills are finished downloading.
	/// </summary>
	public delegate void DownloadsFinished();

	public interface IRunner
	{
		/// <summary>
		/// Run the API downloading and evaluating.
		/// </summary>
		void Run();

		/// <summary>
		/// Handles bills that are downloaded.
		/// </summary>
		/// <param name="bills">The downloaded bills.</param>
		void HandleDownloadedBills(Bill[] bills);

		/// <summary>
		/// Handles when bill downloads are finished.
		/// </summary>
		void HandleDownloadsFinished();

		/// <summary>
		/// Gets the death indicators for a bill.
		/// </summary>
		/// <returns>The indicators.</returns>
		/// <param name="bill">The bill.</param>
		DeathEvaluator.BillDeathIndicators GetIndicators(Bill bill);
	}
}
