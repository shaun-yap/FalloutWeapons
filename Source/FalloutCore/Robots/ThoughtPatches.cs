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
	[HarmonyPatch(typeof(Messages), "Message", new Type[]
	{
		typeof(string),
		typeof(LookTargets),
		typeof(MessageTypeDef),
		typeof(bool)
	})]
	internal static class Message_Patch
	{
		private static bool Prefix(string text, LookTargets lookTargets, MessageTypeDef def)
		{
			if (def == MessageTypeDefOf.PawnDeath && lookTargets.TryGetPrimaryTarget().Thing is Pawn pawn && (pawn.IsRobot()))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn_HealthTracker))]
	[HarmonyPatch("NotifyPlayerOfKilled")]
	internal static class DeadPawnMessageReplacement
	{
		public static bool DisableKilledEffect = false;
		private static bool Prefix(Pawn_HealthTracker __instance, Pawn ___pawn, DamageInfo? dinfo, Hediff hediff, Caravan caravan)
		{
			if (DisableKilledEffect)
			{
				DisableKilledEffect = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_ForHumanlike")]
	public class AppendThoughts_ForHumanlike_Patch
	{
		public static bool DisableKilledEffect = false;

		[HarmonyPrefix]
		public static bool Prefix(ref Pawn victim)
		{
			if (DisableKilledEffect)
			{
				DisableKilledEffect = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PawnDiedOrDownedThoughtsUtility), "AppendThoughts_Relations")]
	public class AppendThoughts_Relations_Patch
	{
		public static bool DisableKilledEffect = false;

		[HarmonyPrefix]
		public static bool Prefix(ref Pawn victim)
		{
			if (DisableKilledEffect)
			{
				DisableKilledEffect = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Faction), "Notify_LeaderDied")]
	public static class Notify_LeaderDied_Patch
	{
		public static bool DisableKilledEffect = false;

		[HarmonyPrefix]
		public static bool Prefix()
		{
			if (DisableKilledEffect)
			{
				DisableKilledEffect = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(StatsRecord), "Notify_ColonistKilled")]
	public static class Notify_ColonistKilled_Patch
	{
		public static bool DisableKilledEffect = false;

		[HarmonyPrefix]
		public static bool Prefix()
		{
			if (DisableKilledEffect)
			{
				DisableKilledEffect = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn_RoyaltyTracker), "Notify_PawnKilled")]
	public static class Notify_PawnKilled_Patch
	{
		public static bool DisableKilledEffect = false;

		[HarmonyPrefix]
		public static bool Prefix()
		{
			if (DisableKilledEffect)
			{
				DisableKilledEffect = false;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	public class Pawn_Kill_Patch
	{
		public static void Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
			try
			{
				if (dinfo.HasValue && dinfo.Value.Def == DamageDefOf.Crush && dinfo.Value.Category == DamageInfo.SourceCategory.Collapse)
				{
					return;
				}
				if (__instance != null && (__instance.IsRobot()))
				{
					Notify_ColonistKilled_Patch.DisableKilledEffect = true;
					Notify_PawnKilled_Patch.DisableKilledEffect = true;
					Notify_LeaderDied_Patch.DisableKilledEffect = true;
					AppendThoughts_ForHumanlike_Patch.DisableKilledEffect = true;
					AppendThoughts_Relations_Patch.DisableKilledEffect = true;
					DeadPawnMessageReplacement.DisableKilledEffect = true;
				}
			}
			catch { };
		}
	}
}

