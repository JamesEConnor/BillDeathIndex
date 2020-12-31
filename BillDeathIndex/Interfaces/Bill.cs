using System;
namespace BillDeathIndex
{
	public abstract class Bill
	{
		/// <summary>
		/// The death level of this bill
		/// </summary>
		public DeathEvaluator.BillDeathLevel deathLevel;
	}
}
