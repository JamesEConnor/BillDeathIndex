using System;
using System.Collections.Generic;

namespace BillDeathIndex.Utils
{
	public class WebRequestUtils
	{
		/// <summary>
		/// Takes a URL and replaces all parameter values with their new values.
		/// </summary>
		/// <returns>The newly constructed URL.</returns>
		/// <param name="oldURL">The old URL that's being changed.</param>
		/// <param name="newValues">The new parameter values to set.</param>
		public static string ConstructNewURL(string oldURL, Dictionary<string, string> newValues)
		{
			int parameterIndex = oldURL.IndexOf('?');
			if (parameterIndex == -1)
				return oldURL;

			foreach (KeyValuePair<string, string> pair in newValues)
			{
				int startIndex = oldURL.IndexOf(pair.Key, parameterIndex, StringComparison.CurrentCultureIgnoreCase);
				int endIndex = oldURL.IndexOf('&', startIndex + 1, StringComparison.CurrentCultureIgnoreCase);
				string suffix = endIndex != -1 ? oldURL.Substring(endIndex) : "";

				oldURL = oldURL.Substring(0, startIndex) + pair.Key + "=" + pair.Value + suffix;
			}

			return oldURL;
		}
	}
}
