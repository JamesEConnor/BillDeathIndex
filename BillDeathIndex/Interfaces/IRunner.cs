using System;

namespace BillDeathIndex
{
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
		/// Gets the death indicators for a bill.
		/// </summary>
		/// <returns>The indicators.</returns>
		/// <param name="bill">The bill.</param>
		DeathEvaluator.BillDeathIndicators GetIndicators(Bill bill);
	}
}
