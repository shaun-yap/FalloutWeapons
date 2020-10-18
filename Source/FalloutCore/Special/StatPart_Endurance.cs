using System;
using RimWorld;
using Verse;

namespace Special
{
	internal class StatPart_Endurance : StatPart
	{
		public override void TransformValue(StatRequest req, ref float val)
		{
			bool hasThing = req.HasThing;
			if (hasThing)
			{
				Pawn pawn = req.Thing as Pawn;
				var comp = pawn.TryGetComp<SpecialComp>();
				if (comp != null)
				{
					val *= 1f + (((float)comp.Endurance - 5f) / 10f) * this.weight;
				}
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			if (req.HasThing)
			{
				Pawn pawn = req.Thing as Pawn;
				if (pawn != null)
				{
					var comp = pawn.TryGetComp<SpecialComp>();
					if (comp != null)
					{
						return Translator.Translate("StatsReport_STAT_Endurance") + 
							": x" + GenText.ToStringPercent(1f + (((float)comp.Endurance - 5f) / 10f) * this.weight);
					}
				}
			}
			return "";
		}

		public float weight;
	}
}

