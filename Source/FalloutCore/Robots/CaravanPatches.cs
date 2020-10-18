using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RobotStuff;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FalloutCore
{
	[HarmonyPatch(typeof(Caravan), "get_NightResting")]
	public class NightResting_Patch
	{
		[HarmonyPostfix]
		public static void Listener(ref bool __result, ref Caravan __instance)
		{
			if (!GenCollection.Any<Pawn>(__instance.pawns.InnerListForReading, (Pawn p) => p.needs.rest != null))
			{
				__result = false;
			}
		}
	}

}

