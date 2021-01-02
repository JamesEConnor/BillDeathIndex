using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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

	/*
	public class SavableBillConverter : JsonConverter<SavableBill>
	{
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:BillDeathIndex.SavableBillConverter"/> can read.
		/// </summary>
		/// <value>Always <c>false</c>.</value>
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:BillDeathIndex.SavableBillConverter"/> can write.
		/// </summary>
		/// <value>Always <c>true</c>.</value>
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override SavableBill ReadJson(JsonReader reader, Type objectType, SavableBill existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		/*
		/// <summary>
		/// Writes a SavableBill object to a JSON string.
		/// </summary>
		/// <param name="writer">The JSON writer to send the string to.</param>
		/// <param name="value">The bill to convert.</param>
		/// <param name="serializer">Serializer.</param>
		public override void WriteJson(JsonWriter writer, SavableBill value, JsonSerializer serializer)
		{
			string JSON = string.Format(
				"\"{0}\":{" +
				"\"printNo\":\"{0}\"," +
				"\"year\":\"{1}\"," +
				"\"title\":\"{2}\"," +
				"\"summary\":\"{3}\"," +
				"\"fullText\":\"{4}\"," +
				"\"signed\":\"{5}\"," +
				"\"adopted\":\"{6}\"," +
				"\"vetoed\":\"{7}\"," +
				"\"deathLevel\":\"{8}\"" +
				"}",
				value.printNo, value.year, value.title, value.summary, value.fullText, value.signed, value.adopted, value.vetoed, value.deathLevel);

			writer.WriteValue(JSON);
		}
	}*/
}
