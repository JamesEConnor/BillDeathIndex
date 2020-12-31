using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using CsvHelper;
using Newtonsoft.Json;

namespace BillDeathIndex.Utils
{
	public class IOUtils
	{
		/// <summary>
		/// Overwrites the contents of the specified file with an empty string.
		/// </summary>
		/// <param name="fullPath">The full path to the file.</param>
		public static void OverwriteFile(string fullPath, string newText)
		{
			File.WriteAllText(fullPath, newText);
		}

		/// <summary>
		/// Ensures the file exists.
		/// </summary>
		/// <returns><c>true</c>, if file was created, <c>false</c> if it already existed.</returns>
		/// <param name="fullPath">The absolute path to the file.</param>
		public static bool EnsureFileExists(string fullPath)
		{
			//Ensure all necessary directories exist.
			Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

			//Checks if the file exists
			if (!File.Exists(fullPath))
			{
				//If it doesn't, create it and close the stream.
				FileStream stream = File.Create(fullPath);
				stream.Close();

				return true;
			}

			return false;
		}

		/// <summary>
		/// Waits for a file to be unlocked.
		/// </summary>
		/// <returns>The file stream for the file.</returns>
		/// <param name="fullPath">The full path to the file.</param>
		/// <param name="mode">The file mode.</param>
		/// <param name="access">The file access.</param>
		/// <param name="share">The file share.</param>
		public static FileStream WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share)
		{
			for (int numTries = 0; numTries < 10; numTries++)
			{
				FileStream fs = null;
				try
				{
					fs = new FileStream(fullPath, mode, access, share);
					return fs;
				}
				catch (IOException)
				{
					if (fs != null)
					{
						fs.Dispose();
					}

					Thread.Sleep(50);
				}
			}

			return null;
		}

		/// <summary>
		/// Converts a JSON string to a CSV entry.
		/// </summary>
		/// <returns>The csv lines.</returns>
		/// <param name="jsonContent">The JSON content.</param>
		/// <param name="delimiter">The CSV delimiter.</param>
		public static string JSONToCSV(string jsonContent, string delimiter)
		{
			StringWriter csvString = new StringWriter();
			using (var csv = new CsvWriter(csvString, System.Globalization.CultureInfo.CurrentCulture))
			{
				csv.Configuration.Delimiter = delimiter;

				using (var dt = JSONStringToTable(jsonContent))
				{
					//Removed as the header isn't necessary
					/*foreach (DataColumn column in dt.Columns)
					{
						csv.WriteField(column.ColumnName);
					}
					csv.NextRecord();*/

					foreach (DataRow row in dt.Rows)
					{
						for (var i = 0; i < dt.Columns.Count; i++)
						{
							csv.WriteField(row[i]);
						}
						csv.NextRecord();
					}
				}
			}

			return csvString.ToString();
		}

		/// <summary>
		/// Converts a JSON string to a DataTable.
		/// </summary>
		/// <returns>The datatable.</returns>
		/// <param name="jsonContent">The JSON string.</param>
		private static DataTable JSONStringToTable(string jsonContent)
		{
			try
			{
				string result = Regex.Replace(jsonContent, @"[\x00\x08\x0B\x0C\x0E-\x1F]+", string.Empty, RegexOptions.Singleline);

				Console.WriteLine(result);

				DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);
				return dt;
			}
			catch(Exception e)
			{
				Logger.LogError(e);
				throw e;
			}
		}
	}
}
