using Verse;

namespace FalloutCore
{
	public class HediffGiver_RandomWithChance : HediffGiver
	{
		public float mtbDays;
		public float chance;
		public override void OnIntervalPassed(Pawn pawn, Hediff cause)
		{
			if (Rand.MTBEventOccurs(mtbDays, 60000f, 60f) && Rand.Range(0f, 100f) <= chance * 100f && TryApply(pawn))
			{
				SendLetter(pawn, cause);
			}
		}
	}
}
