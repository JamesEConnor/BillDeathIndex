using System;
using System.IO;
using BillDeathIndex;
using BillDeathIndex.Converters;
using BillDeathIndex.Utils;
using Newtonsoft.Json;

namespace BillDeathIndex.States.NY
{
	public class NYSConverter : ICSVConverter
	{
		/// <summary>
		/// The path to save results to.
		/// </summary>
		public static string RESULTS_PATH
		{
			get
			{
				return System.IO.Path.GetFullPath("files/states/ny.csv");
			}
		}

		public NYSConverter()
		{
			//Overwrites any previous file contents.
			IOUtils.OverwriteFile(RESULTS_PATH, string.Empty);
		}

		/// <summary>
		/// Converts a bill in JSON format to a bill in CSV format.
		/// </summary>
		/// <param name="JSON">The JSON to convert.</param>
		public string Convert(string JSON)
		{
			return Convert(JsonConvert.DeserializeObject<NYSBill>(JSON));
		}

		/// <summary>
		/// Converts a bill object to a CSV string.
		/// </summary>
		/// <param name="bill">The bill to convert.</param>
		public string Convert(Bill bill)
		{
			NYSBill nysBill = (NYSBill)bill;

			return string.Join(",", new string[] {
				nysBill.printNo,
				nysBill.title,
				nysBill.summary,
				nysBill.deathLevel.ToString()
			});
		}

		/*public string Convert(Bill bill)
		{
			//Get the bill
			NYSBill nysBill = (NYSBill)bill;

			string result = "";

			//Add data points
			result += nysBill.printNo + ",";
			result += nysBill.session + ",";
			result += nysBill.billType.chamber + ",";
			result += nysBill.billType.resolution + ",";
			result += nysBill.title + ",";
			result += nysBill.publishedDateTime + ",";

			//Add sponsor details
			if (nysBill.sponsor != null && nysBill.sponsor.member != null)
			{
				result += nysBill.sponsor.member.fullName + ",";
				result += nysBill.sponsor.member.districtCode + ",";
				result += nysBill.sponsor.member.chamber + ",";
				result += nysBill.sponsor.member.incumbent + ",";
			}
			else
				result += ",,,,";

			result += nysBill.summary + ",";
			result += nysBill.signed + ",";
			result += nysBill.adopted + ",";
			result += nysBill.vetoed + ",";
			result += nysBill.status.statusDesc + ",";
			result += nysBill.status.actionDate + ",";
			result += nysBill.status.committeeName + ",";

			result += "[";
			for (int a = 0; a < nysBill.milestones.items.Length; a++)
			{
				result += "{";
				result += nysBill.milestones.items[a].statusDesc + ",";
				result += nysBill.milestones.items[a].actionDate + ",";
				result += nysBill.milestones.items[a].committeeName;
				result += "}" + (a < nysBill.milestones.items.Length - 1 ? "," : "");
			}
			result += "],";

			result += "[";
			for (int a = 0; a < nysBill.actions.items.Length; a++)
			{
				result += "{";
				result += nysBill.actions.items[a].billId + ",";
				result += nysBill.actions.items[a].sequenceNo + ",";
				result += nysBill.actions.items[a].text + ",";
				result += nysBill.actions.items[a].chamber + ",";
				result += nysBill.actions.items[a].date;
				result += "}" + (a < nysBill.actions.items.Length - 1 ? "," : "");
			}
			result += "],";

			result += nysBill.amendments.items[""].memo + ",";
			result += nysBill.amendments.items[""].lawSection + ",";
			result += nysBill.amendments.items[""].lawCode + ",";
			result += nysBill.amendments.items[""].fullText + ",";

			NYSRepresentative[] cosponsors = nysBill.amendments.items[""].coSponsors.items;

			result += "[";
			for (int a = 0; a < cosponsors.Length; a++)
			{
				result += "{";
				result += cosponsors[a].fullName + ",";
				result += cosponsors[a].districtCode + ",";
				result += cosponsors[a].chamber + ",";
				result += cosponsors[a].incumbent;
				result += "}" + (a < cosponsors.Length - 1 ? "," : "");
			}
			result += "],";

			result += nysBill.deathLevel;
		}*/

		/// <summary>
		/// Generates a header line.
		/// </summary>
		public string GetHeaderLine()
		{
			return string.Join(",", new string[] {
				"printNo",
				"title",
				"summary",
				"deathLevel"
			});
		}

		/*public string GetHeaderLine()
		{
			return string.Join(",", new string[] {
				"printNo",
				"session",
				"chamber",
				"resolution",
				"title",
				"publishedDatetime",
				"sponsorName",
				"sponsorDistrict",
				"sponsorChamber",
				"sponsorIncumbent",
				"summary",
				"signed",
				"adopted",
				"vetoed",
				"status",
				"statusDate",
				"statusCommittee",
				"milestones",
				"actions",
				"amendmentMemo",
				"amendmentLawSection",
				"amendmentLawCode",
				"amendmentText",
				"cosponsors",
				"deathLevel"
			});
		}*/

		/// <summary>
		/// Saves the header line to the CSV file.
		/// </summary>
		public void SaveHeaderLine()
		{
			//Ensure the file exists
			IOUtils.EnsureFileExists(RESULTS_PATH);

			//Write the line to the file.
			using (var fs = new FileStream(RESULTS_PATH, FileMode.Append, FileAccess.Write, FileShare.Write))
			{
				using (var writer = new StreamWriter(fs))
				{
					writer.WriteLine(GetHeaderLine());
				}
			}
		}

		/// <summary>
		/// Converts the JSON to a CSV format and appends it to a file.
		/// </summary>
		/// <param name="JSON">The JSON to convert.</param>
		public void SaveConversion(string JSON)
		{
			SaveConversion(JsonConvert.DeserializeObject<NYSBill>(JSON));
		}

		/// <summary>
		/// Converts the bill to a CSV format and appends it to a file.
		/// </summary>
		/// <param name="bill">The bill to convert.</param>
		public void SaveConversion(Bill bill)
		{
			//Ensure the file exists
			IOUtils.EnsureFileExists(RESULTS_PATH);

			//Convert the bill.
			string line = Convert(bill);

			//Write the line to the file.
			using (var fs = new FileStream(RESULTS_PATH, FileMode.Append, FileAccess.Write, FileShare.Write))
			{
				using (var writer = new StreamWriter(fs))
				{
					writer.WriteLine(line);
				}
			}
		}
	}
}
