using System;
using Newtonsoft.Json;

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
	//[JsonConverter(typeof(SavableBillConverter))]
	public struct SavableBill
	{
		/// <summary>
		/// The print number of the bill.
		/// </summary>
		public string printNo;

		/// <summary>
		/// The year that the bill is from.
		/// </summary>
		public int year;

		/// <summary>
		/// The title of the bill.
		/// </summary>
		public string title;

		/// <summary>
		/// A summary of the bill.
		/// </summary>
		public string summary;

		/// <summary>
		/// The full text of the bill.
		/// </summary>
		public string fullText;

		/// <summary>
		/// Whether the bill was signed into law.
		/// </summary>
		public bool signed;

		/// <summary>
		/// Whether the bill was adopted or not.
		/// </summary>
		public bool adopted;

		/// <summary>
		/// Whether the bill was vetoed or not.
		/// </summary>
		public bool vetoed;

		/// <summary>
		/// The death level for the bill.
		/// </summary>
		public DeathEvaluator.BillDeathLevel deathLevel;

		public SavableBill(
			string _printNo,
			int _year,
			string _title,
			string _summary,
			string _fullText,
			bool _signed,
			bool _adopted,
			bool _vetoed,
			DeathEvaluator.BillDeathLevel _deathLevel)
		{
			printNo = _printNo;
			year = _year;
			title = _title;
			summary = _summary;
			fullText = _fullText;
			signed = _signed;
			adopted = _adopted;
			vetoed = _vetoed;
			deathLevel = _deathLevel;
		}
	}
}
