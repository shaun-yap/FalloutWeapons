using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;

namespace FalloutCore
{
	public class StatWorker_AdditionalArmorStats : StatWorker
	{
		public override bool ShouldShowFor(StatRequest req)
		{
			ThingDef thingDef = req.Def as ThingDef;
			return base.ShouldShowFor(req) && thingDef != null && thingDef.HasModExtension<AdditionalArmorProtection>();
		}

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
			ThingDef thingDef = optionalReq.Def as ThingDef;
			var option = thingDef.GetModExtension<AdditionalArmorProtection>();
			var bodyParts = new List<string>();
			if (option != null)
			{
				foreach (var stat2 in option.additionalArmors)
				{
					bodyParts.Add(stat2.bodyPart.LabelCap);
				}
			}
			return string.Join(", ", bodyParts);
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            return "";
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ThingDef thingDef = req.Def as ThingDef;
			var option = thingDef.GetModExtension<AdditionalArmorProtection>();
			if (option != null)
            {
				foreach (var stat in option.additionalArmors)
                {
					var str = stat.bodyPart.LabelCap;
					foreach (var statModifiers in stat.ArmorStats)
                    {
						str += " - " + statModifiers.stat.LabelCap.Replace("Armor - ", "") + ": " + statModifiers.value;
                    }
					stringBuilder.AppendLine(str);
				}
            }
			return stringBuilder.ToString();
		}
	}
}

