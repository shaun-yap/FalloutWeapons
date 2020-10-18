using System;
using System.Text;
using RimWorld;
using Verse;

namespace Special
{
	public class StatWorker_Strength : StatWorker 
	{

		public override bool ShouldShowFor(StatRequest req)
		{
			ThingDef thingDef = req.Def as ThingDef;
			return base.ShouldShowFor(req) && thingDef != null && thingDef.race != null && thingDef.race.Humanlike;
		}

		public override void FinalizeValue(StatRequest req, ref float val, bool applyPostProcess)
		{
			Pawn pawn = (Pawn)req.Thing;
			var comp = pawn.TryGetComp<SpecialComp>();
			if (comp != null) val = comp.Strength;
			base.FinalizeValue(req, ref val, applyPostProcess);
		}

		public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.GetExplanationFinalizePart(req, numberSense, finalVal));
			return stringBuilder.ToString();
		}
	}
}

