using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace BillDeathIndex.Utils
{
	public class Secrets
	{
		/// <summary>
		/// The credentials.
		/// </summary>
		private static Credential[] credentials;

		/// <summary>
		/// Initialize the credentials object.
		/// </summary>
		private static void Initialize()
		{
			//Load the JSON from a file and deserialize it.
			string json = File.ReadAllText(Path.GetFullPath("files/creds.json"));
			credentials = JsonConvert.DeserializeObject<Credential[]>(json);

			foreach (Credential c in credentials)
				Logger.Log(c.ToString());
		}

		/// <summary>
		/// Gets the credential object for the specified application.
		/// </summary>
		/// <returns>The credential object.</returns>
		/// <param name="application">The application to get credentials for.</param>
		public static Credential GetCredential(string application)
		{
			//Ensure the credentials have been identified.
			if (credentials == null)
				Initialize();

			//Get the credentials object
			return credentials.First(credential => credential.appName == application);
		}
	}

	public struct Credential
	{
		//The application that these credentials are for.
		public string appName;

		//The various credentials.
		public Dictionary<string, string> creds;

		public override string ToString()
		{
			string result = "[Credential=" + appName + "]";

			foreach (KeyValuePair<string, string> pair in creds)
				result += string.Format("[{0}={1}]", pair.Key, pair.Value);

			return result;
		}
	}
}
