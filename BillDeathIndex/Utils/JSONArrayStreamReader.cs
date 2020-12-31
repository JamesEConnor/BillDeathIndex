using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BillDeathIndex
{
	public class JSONArrayStreamReader : IEnumerable<string>, IDisposable
	{
		/// <summary>
		/// The file stream to read from.
		/// </summary>
		FileStream fileStream;

		/// <summary>
		/// The reader for the file.
		/// </summary>
		StreamReader reader;

		/// <summary>
		/// Tracks whether the object was disposed, to cancel the iterator.
		/// </summary>
		bool endedStream = false;

		public JSONArrayStreamReader(string filePath)
		{
			fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			reader = new StreamReader(fileStream);
		}

		int counter = 0;

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<string> GetEnumerator()
		{
			while (!reader.EndOfStream && !endedStream)
			{
				string result = "";

				//Start the current read with an open bracket.
				while (result != "{")
				{
					result = ((char)reader.Read()).ToString();
				}

				//Ensures that all parts of the JSON object are captured.
				int bracketLevel = 1;
				//If inside quotes, don't count brackets towards the bracket level.
				bool insideQuotes = false;
				bool escapeChar = false;

				while (bracketLevel > 0)
				{
					//Read the line
					char ch = (char)reader.Read();

					if (ch == '"' && !escapeChar)
						insideQuotes = !insideQuotes;

					//Track the nesting level
					if (!insideQuotes)
					{
						if (ch == '{')
							bracketLevel++;
						else if (ch == '}')
							bracketLevel--;
					}
					else
					{
						if (escapeChar)
							escapeChar = false;

						if (ch == '\\')
							escapeChar = true;
					}

					//Add the line to the final result.
					result += ch;
				}

				counter++;
				yield return result;
			}
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="T:BillDeathIndex.JSONArrayStreamReader"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:BillDeathIndex.JSONArrayStreamReader"/>.
		/// The <see cref="Dispose"/> method leaves the <see cref="T:BillDeathIndex.JSONArrayStreamReader"/> in an unusable
		/// state. After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="T:BillDeathIndex.JSONArrayStreamReader"/> so the garbage collector can reclaim the memory that the
		/// <see cref="T:BillDeathIndex.JSONArrayStreamReader"/> was occupying.</remarks>
		public void Dispose()
		{
			endedStream = true;

			fileStream.Close();
			fileStream.Dispose();

			reader.Close();
			reader.Dispose();
		}
	}
}
