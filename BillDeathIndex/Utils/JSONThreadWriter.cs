using System;
using System.IO;
using System.Text;

namespace BillDeathIndex
{
	public class JSONThreadWriter : StreamWriter
	{
		bool firstLine = true;

		//Constructors
		public JSONThreadWriter(string path, bool append, Encoding encoding, int bufferSize) : base(path, append, encoding, bufferSize) { }

		public JSONThreadWriter(string path, bool append) : base(path, append) { }

		public JSONThreadWriter(string path) : base(path) { }

		public JSONThreadWriter(string path, bool append, Encoding encoding) : base(path, append, encoding) { }

		public JSONThreadWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize) { }

		public JSONThreadWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }

		public JSONThreadWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen) : base(stream, encoding, bufferSize, leaveOpen) { }

		public JSONThreadWriter(Stream stream) : base(stream) { }

		/// <summary>
		/// Writes a line to the file.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="includeComma">Whether to tack a comma onto the beginning.</param>
		public void WriteLine(string value, bool includeComma=true)
		{
			//Add commas to all but the first entry.
			if (!firstLine && includeComma)
				value = "," + value;
			else
				firstLine = false;

			//Write the line
			base.WriteLine(value);
		}
	}
}
