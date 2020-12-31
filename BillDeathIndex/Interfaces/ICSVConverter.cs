using System;

using BillDeathIndex;

namespace BillDeathIndex.Converters
{
	public interface ICSVConverter
	{
		/// <summary>
		/// Converts a bill in JSON format to a bill in CSV format.
		/// </summary>
		/// <param name="JSON">The JSON to convert.</param>
		string Convert(string JSON);

		/// <summary>
		/// Converts a bill object to a CSV string.
		/// </summary>
		/// <param name="bill">The bill to convert.</param>
		string Convert(Bill bill);

		/// <summary>
		/// Generates a header line.
		/// </summary>
		string GetHeaderLine();

		/// <summary>
		/// Saves the header line to the CSV file.
		/// </summary>
		void SaveHeaderLine();

		/// <summary>
		/// Converts the JSON to a CSV format and appends it to a file.
		/// </summary>
		/// <param name="JSON">The JSON to convert.</param>
		void SaveConversion(string JSON);

		/// <summary>
		/// Converts the bill to a CSV format and appends it to a file.
		/// </summary>
		/// <param name="bill">The bill to convert.</param>
		void SaveConversion(Bill bill);
	}
}
