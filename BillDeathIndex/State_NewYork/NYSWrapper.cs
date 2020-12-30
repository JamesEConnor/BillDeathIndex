using System;
using System.Collections.Generic;

namespace BillDeathIndex.States.NY
{
	public class NYSBill
	{
		/// <summary>
		/// The print number of the bill.
		/// </summary>
		public string printNo;

		/// <summary>
		/// The year that the bill was introduced.
		/// </summary>
		public int session;

		/// <summary>
		/// Details about the bill's origin and type.
		/// </summary>
		public NYSBillType billType;

		public struct NYSBillType
		{
			/// <summary>
			/// The chamber in which the bill was introduced.
			/// </summary>
			public string chamber;

			/// <summary>
			/// A description of the bill type.
			/// </summary>
			public string desc;

			/// <summary>
			/// Whether its a bill (false) or resolution (true).
			/// </summary>
			public bool resolution;
		}

		/// <summary>
		/// The title of the bill.
		/// </summary>
		public string title;

		/// <summary>
		/// The active version of the bill.
		/// </summary>
		public string activeVersion;

		public int year;

		/// <summary>
		/// The date and time when the bill was published.
		/// </summary>
		public DateTime publishedDateTime;

		/// <summary>
		/// A bill that substituted this one.
		/// </summary>
		public NYSBill substitutedBy; //TODO: CONFIRM THIS

		/// <summary>
		/// The sponsor of this bill.
		/// </summary>
		public NYSSponsor sponsor;

		/// <summary>
		/// Represents the sponsor of a bill.
		/// </summary>
		public struct NYSSponsor
		{
			/// <summary>
			/// The NYS Representative acting as sponsor for this bill.
			/// </summary>
			public NYSRepresentative member;

			/// <summary>
			/// Whether this sponsor is part of the budget committee.
			/// </summary>
			public bool budget;

			/// <summary>
			/// Whether this sponsor is part of the rules committee.
			/// </summary>
			public bool rules;
		}

		/// <summary>
		/// A summary of the bill.
		/// </summary>
		public string summary;

		/// <summary>
		/// Whether this bill was signed.
		/// </summary>
		public bool signed;

		/// <summary>
		/// Whether this bill was adopted.
		/// </summary>
		public bool adopted;

		/// <summary>
		/// Whether this bill was vetoed.
		/// </summary>
		public bool vetoed;

		/// <summary>
		/// The current status of the bill.
		/// </summary>
		public NYSBillStatus status;

		public struct NYSBillStatus
		{
			/// <summary>
			/// The status of the bill.
			/// </summary>
			public string statusType;

			/// <summary>
			/// A more specific description of the bill's status.
			/// </summary>
			public string statusDesc;

			/// <summary>
			/// When the status was last changed.
			/// </summary>
			public DateTime actionDate;

			/// <summary>
			/// The committee this bill resides in, if applicable.
			/// </summary>
			public string committeeName;
		}

		/// <summary>
		/// Milestone statuses for the bill.
		/// </summary>
		public NYSMilestones milestones;

		public struct NYSMilestones
		{
			/// <summary>
			/// The number of milestones.
			/// </summary>
			public int size;

			/// <summary>
			/// The status milestones.
			/// </summary>
			public NYSBillStatus[] items;
		}

		/// <summary>
		/// A collection of the actions taken on this bill.
		/// </summary>
		public NYSBillActions actions;

		public struct NYSBillActions
		{
			/// <summary>
			/// The number of actions.
			/// </summary>
			public int size;

			/// <summary>
			/// The actions taken on the bill.
			/// </summary>
			public NYSBillAction[] items;

			public struct NYSBillAction
			{
				/// <summary>
				/// Identifies the bill.
				/// </summary>
				public NYSBillId billId;

				/// <summary>
				/// The date this action was taken.
				/// </summary>
				public DateTime date;

				/// <summary>
				/// The chamber in which this action was taken.
				/// </summary>
				public string chamber;

				/// <summary>
				/// The sequence number of this action. (i.e. where it lies in the order)
				/// </summary>
				public int sequenceNo;

				/// <summary>
				/// Text describing the action.
				/// </summary>
				public string text;
			}
		}

		public struct NYSBillAmendments
		{
			/// <summary>
			/// The number of amendments that were made
			/// </summary>
			public int size;

			/// <summary>
			/// The bill amendments.
			/// </summary>
			public NYSBillAmendmentParent items;

			//Handles an issue with how JSON is delivered 
			public class NYSBillAmendmentParent : Dictionary<string, NYSBillAmendment> {}

			public struct NYSBillAmendment
			{
				/// <summary>
				/// The print number of the bill.
				/// </summary>
				public string printNo;

				/// <summary>
				/// The year that the bill was introduced.
				/// </summary>
				public int session;

				/// <summary>
				/// The version of the bill.
				/// </summary>
				public string version;

				/// <summary>
				/// The date that the amendment was published.
				/// </summary>
				public DateTime publishDate;

				/// <summary>
				/// Bills from either the same or opposite chamber that's the same as this bill.
				/// </summary>
				public NYSSameBills sameAs;

				public struct NYSSameBills
				{
					/// <summary>
					/// The number of bills that are the same as this amendment.
					/// </summary>
					public int size;

					/// <summary>
					/// The details of each bill that's the same.
					/// </summary>
					public NYSBillId[] items;
				}

				/// <summary>
				/// The summary of the important points of the bill, but more in depth than the summary.
				/// </summary>
				public string memo;

				/// <summary>
				/// The section of law this amendment concerns.
				/// </summary>
				public string lawSection;

				/// <summary>
				/// The law code this amendment concerns.
				/// </summary>
				public string lawCode;

				/// <summary>
				/// Describes the act.
				/// </summary>
				public string actClause;

				/// <summary>
				/// The full text of the amendment.
				/// </summary>
				public string fullText;

				/// <summary>
				/// The amendment's co-sponsors.
				/// </summary>
				public NYSCosponsors coSponsors;

				public struct NYSCosponsors
				{
					/// <summary>
					/// The number of co-sponsors.
					/// </summary>
					public int size;

					/// <summary>
					/// The co-sponsors.
					/// </summary>
					public NYSRepresentative[] items;
				}
			}
		}
	}

	/// <summary>
	/// The response returned by a request to the NYS API.
	/// </summary>
	public struct NYSBillResponse
	{
		/// <summary>
		/// Whether or not the request was a success.
		/// </summary>
		public bool success;

		/// <summary>
		/// A status message, if one exists.
		/// </summary>
		public string message;

		/// <summary>
		/// The type of response given.
		/// </summary>
		public string responseType;

		/// <summary>
		/// The total number of bills that match the request.
		/// </summary>
		public int total;

		/// <summary>
		/// The first bill number, where the response started.
		/// </summary>
		public int offsetStart;

		/// <summary>
		/// The last bill number, where the response ended.
		/// </summary>
		public int offsetEnd;

		/// <summary>
		/// The maximum number of bills that was allowed.
		/// </summary>
		public int limit;

		/// <summary>
		/// The result.
		/// </summary>
		public NYSBillResult result;

		public struct NYSBillResult
		{
			/// <summary>
			/// The number of bills returned.
			/// </summary>
			public int size;

			/// <summary>
			/// The actual bills.
			/// </summary>
			public NYSBill[] items;
		}
	}

	/// <summary>
	/// A struct for representing a member of the NYS Senate/Assembly.
	/// </summary>
	public struct NYSRepresentative
	{
		/// <summary>
		/// The member ID of the rep.
		/// </summary>
		int memberId;

		/// <summary>
		/// The chamber of the rep.
		/// </summary>
		public string chamber;

		/// <summary>
		/// Whether this representative is the incumbent.
		/// </summary>
		public bool incumbent;

		/// <summary>
		/// The full name of the rep.
		/// </summary>
		public string fullName;

		/// <summary>
		/// The district number of the rep.
		/// </summary>
		public int districtCode;
	}



	/// <summary>
	/// Identifies the bill involved.
	/// </summary>
	public struct NYSBillId
	{
		/// <summary>
		/// The print number of the bill.
		/// </summary>
		public string printNo;

		/// <summary>
		/// The year that the bill was introduced.
		/// </summary>
		public int session;

		/// <summary>
		/// The version of the bill.
		/// </summary>
		public string version;
	}
}
