using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace FalloutCore
{
	public class FactionDialogMaker_Patch
	{
		[HarmonyPatch(typeof(FactionDialogMaker), "FactionDialogFor")]
		public class FactionDialogFor
		{
			public static void Postfix(Pawn negotiator, Faction faction, ref DiaNode __result)
			{
				if (__result != null && faction.def.HasModExtension<CallOptions>())
				{
					var options = faction.def.GetModExtension<CallOptions>();
					var diaOptions = new List<DiaOption>();
					foreach (var option in options.options)
					{
						DiaOption diaOption = new DiaOption(option.text);
						diaOption.action = delegate()
						{
							IncidentParms incidentParms = new IncidentParms();
							incidentParms.target = negotiator.Map;
							incidentParms.faction = faction;
							if (option.callIncidentDef != null)
							{
								if (option.callIncidentDef.NeedsParmsPoints)
								{
									incidentParms.points = StorytellerUtility.DefaultThreatPointsNow(negotiator.Map) * 1.2f;
								}
								option.callIncidentDef.Worker.TryExecute(incidentParms);
							}
							Messages.Message(option.message, MessageTypeDefOf.NeutralEvent, true);
							faction.TryAffectGoodwillWith(negotiator.Faction, option.goodwillCost);
						};
						diaOptions.Add(diaOption);
					}
					__result.options.InsertRange(__result.options.Count - 1, diaOptions);
				}
			}
		}
	}
}

