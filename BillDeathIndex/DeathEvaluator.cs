using System;
namespace BillDeathIndex
{
	public class DeathEvaluator
	{
		/// <summary>
		/// The death indicators of a Bill.
		/// </summary>
		[Flags]
		public enum BillDeathIndicators
		{
			None					 = 0,
			Inactivity_OneYear		 = 1,
			Inactivity_FourMonths	 = 2,
			Approval_Committee		 = 4,
			Approval_Chamber		 = 8,
			Referred_Committee		 = 16,
			Vetoed_Pocket			 = 32,
			Vetoed					 = 64,
			Sponsors_Majority		 = 128,
			Sponsors_All			 = 256
		}

		/// <summary>
		/// Bill death level.
		/// </summary>
		public enum BillDeathLevel
		{
			Alive			= 0,
			MostlyDead		= 1,
			Flatlining		= 2,
			Clinical		= 3,
			SixFeetUnder	= 4
		}

		/// <summary>
		/// Evaluates the bill death level based on a bill's death indicators.
		/// </summary>
		/// <returns>The death level.</returns>
		/// <param name="indicators">The indicators for the bill.</param>
		public static BillDeathLevel EvaluateIndicators(BillDeathIndicators indicators)
		{
			//Most stringent death level, with the most conditions to meet.
			if (indicators == (
				BillDeathIndicators.Referred_Committee &
				BillDeathIndicators.Inactivity_FourMonths &
				(
					(
						BillDeathIndicators.Inactivity_OneYear &
						BillDeathIndicators.Sponsors_Majority
					) | BillDeathIndicators.Sponsors_All
				)
			))
				return BillDeathLevel.SixFeetUnder;

			//If a pocket veto or veto with subsequent inactivity occurred, it meets the third level of bill death.
			if (indicators == (
				BillDeathIndicators.Vetoed_Pocket |
				(
					BillDeathIndicators.Inactivity_OneYear &
					BillDeathIndicators.Vetoed
				)
			))
				return BillDeathLevel.Clinical;

			//If last referred to a committee and inactive for a year, it's the second death level.
			if (indicators == (
				BillDeathIndicators.Referred_Committee & 
				BillDeathIndicators.Inactivity_OneYear
			))
				return BillDeathLevel.Flatlining;

			//If last approved by a chamber/committee and inactive for a year, it's the first death level.
			if (indicators == (
				(
					BillDeathIndicators.Approval_Chamber | 
					BillDeathIndicators.Approval_Committee
				) & BillDeathIndicators.Inactivity_OneYear
			))
				return BillDeathLevel.MostlyDead;

			//Otherwise, the bill isn't dead
			return BillDeathLevel.Alive;
		}
	}
}
