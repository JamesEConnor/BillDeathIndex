using System;
namespace BillDeathIndex
{
	public abstract class Bill
	{
		/// <summary>
		/// The death level of this bill
		/// </summary>
		public DeathEvaluator.BillDeathLevel deathLevel;
	}

	/// <summary>
	/// A struct for saving limited information about a bill.
	/// </summary>
	public struct SavableBill
	{
		/// <summary>
		/// The print number of the bill.
		/// </summary>
		public string printNo;

		/// <summary>
		/// The title of the bill.
		/// </summary>
		public string title;

		/// <summary>
		/// A summary of the bill.
		/// </summary>
		public string summary;

		/// <summary>
		/// The death level for the bill.
		/// </summary>
		public DeathEvaluator.BillDeathLevel deathLevel;

		public SavableBill(string _printNo, string _title, string _summary, DeathEvaluator.BillDeathLevel _deathLevel)
		{
			printNo = _printNo;
			title = _title;
			summary = _summary;
			deathLevel = _deathLevel;
		}
	}
}
