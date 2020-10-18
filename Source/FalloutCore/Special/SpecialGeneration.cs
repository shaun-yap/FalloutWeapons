using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Special
{
	public static class SpecialGeneration
	{
        public static void StartGeneration(Pawn pawn)
        {
            if (pawn.TryGetComp<SpecialComp>() == null)
            {
                var comp = new SpecialComp();
                comp.parent = pawn;
                comp.Initialize(null);
                try
                {
                    DoSpecialGeneration(comp);
                }
                catch (Exception ex)
                {
                    if (Scribe.mode != LoadSaveMode.LoadingVars && Scribe.mode != LoadSaveMode.PostLoadInit)
                    {
                        Log.Error("DoSpecialGeneration returned the error on the pawn: " + pawn + " - error: " + ex);
                    }
                }
                pawn.AllComps.Add(comp);
            }
        }

        public static int AdjustSkillPoints(ref int startingPoints, SpecialComp comp, SkillDef skill)
        {
            if (skill == SkillDefOf.Shooting || skill == SkillDefOf.Cooking || skill == SkillDefOf.Crafting)
            {
                if (comp.Perception < 10)
                {
                    comp.Perception += 1;
                    startingPoints += 1;
                }
            }
            else if (skill == SkillDefOf.Melee || skill == SkillDefOf.Mining)
            {
                if (comp.Strength < 10)
                {
                    comp.Strength += 1;
                    startingPoints += 1;
                }
            }
            else if (skill == SkillDefOf.Animals || skill == SkillDefOf.Social || skill == SkillDefOf.Artistic)
            {
                if (comp.Charisma < 10)
                {
                    comp.Charisma += 1;
                    startingPoints += 1;
                }
            }
            else if (skill == SkillDefOf.Intellectual || skill == SkillDefOf.Medicine)
            {
                if (comp.Intelligence < 10)
                {
                    comp.Intelligence += 1;
                    startingPoints += 1;
                }
            }
            else if (skill == SkillDefOf.Plants || skill == SkillDefOf.Construction)
            {
                if (comp.Endurance < 10)
                {
                    comp.Endurance += 1;
                    startingPoints += 1;
                }
            }
            return startingPoints;
        }
        public static void DoSpecialGeneration(SpecialComp comp)
        {
            Pawn pawn = (Pawn)comp.parent;
            int hardCap = 35;
            int startingPoints = 0;
            var childhood = pawn.story.childhood;

            if (childhood != null)
            {
                foreach (var skill in childhood?.skillGainsResolved)
                {
                    if (skill.Value > 0 && startingPoints < hardCap)
                    {
                        AdjustSkillPoints(ref startingPoints, comp, skill.Key);
                    }
                }
            }
            var adulthood = pawn.story.adulthood;
            if (adulthood != null)
            {
                foreach (var skill in adulthood?.skillGainsResolved)
                {
                    if (skill.Value > 0 && startingPoints < hardCap)
                    {
                        AdjustSkillPoints(ref startingPoints, comp, skill.Key);
                    }
                }
            }
            foreach (var skill in pawn.skills.skills)
            {
                if (startingPoints < hardCap)
                {
                    if (skill.passion == Passion.Major)
                    {
                        AdjustSkillPoints(ref startingPoints, comp, skill.def);
                        AdjustSkillPoints(ref startingPoints, comp, skill.def);
                    }
                    else if (skill.passion == Passion.Minor)
                    {
                        AdjustSkillPoints(ref startingPoints, comp, skill.def);
                    }
                }
            }
            foreach (var trait in pawn.story.traits.allTraits)
            {
                foreach (var skill in trait.CurrentData.skillGains)
                {
                    if (skill.Value > 0 && startingPoints < hardCap)
                    {
                        AdjustSkillPoints(ref startingPoints, comp, skill.Key);
                    }
                }
            }
            int randomPoints = hardCap - startingPoints;
            while (randomPoints > 0)
            {
                Random random = new Random(randomPoints + pawn.thingIDNumber);
                switch (random.Next(1, 8))
                {
                    case 1:
                        comp.Strength += 1;
                        randomPoints -= 1;
                        break;
                    case 2:
                        comp.Endurance += 1;
                        randomPoints -= 1;
                        break;
                    case 3:
                        comp.Agility += 1;
                        randomPoints -= 1;
                        break;
                    case 4:
                        comp.Charisma += 1;
                        randomPoints -= 1;
                        break;
                    case 5:
                        comp.Luck += 1;
                        randomPoints -= 1;
                        break;
                    case 6:
                        comp.Perception += 1;
                        randomPoints -= 1;
                        break;
                    case 7:
                        comp.Intelligence += 1;
                        randomPoints -= 1;
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(Thing))]
		[HarmonyPatch("ExposeData")]
		public static class Patch_ExposeData
        {
			[HarmonyPostfix]
			public static void Postfix(Thing __instance)
			{
				if (__instance is Pawn pawn && pawn.RaceProps.Humanlike)
				{
                    SpecialGeneration.StartGeneration(pawn);
				}
			}
        }

        [HarmonyPatch(typeof(Thing))]
        [HarmonyPatch("SpawnSetup")]
        public static class Patch_SpawnSetup
        {
            [HarmonyPostfix]
            public static void Postfix(Thing __instance)
            {
                if (__instance is Pawn pawn && pawn.RaceProps.Humanlike)
                {
                    SpecialGeneration.StartGeneration(pawn);
                }
            }
        }
    }
}

