using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Quests;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace FalloutCore
{
    [StaticConstructorOnStartup]
    public static class VanillaStuffRemoval
    {
        static VanillaStuffRemoval()
        {
            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading
                .Where(d => d.modContentPack.PackageId == ModContentPack.CoreModPackageId).ToList();
            things.AddRange(DefDatabase<ThingDef>.AllDefsListForReading
                .Where(d => d.modContentPack.PackageId == ModContentPack.RoyaltyModPackageId).ToList());    
            foreach (var thing in things)
            {
                if (thing.IsWeapon)
                { 
                    thing.researchPrerequisites?.Clear();
                    thing.weaponTags?.Clear();
                    thing.deepCommonality = 0;
                    thing.generateCommonality = 0;
                    thing.tradeability = Tradeability.None;
                    thing.thingCategories?.Clear();
                    thing.thingCategories?.Add(ThingCategoryDefOf.Chunks);
                } 
            }
            foreach (var recipe in DefDatabase<RecipeDef>.AllDefsListForReading)
            {
                if (recipe.ProducedThingDef != null && recipe.ProducedThingDef.IsWeapon && (recipe.ProducedThingDef?.modContentPack?.PackageId == ModContentPack.CoreModPackageId
                    || recipe.ProducedThingDef?.modContentPack?.PackageId == ModContentPack.RoyaltyModPackageId))
                {
                    recipe.recipeUsers?.Clear();
                    recipe.researchPrerequisites?.Clear();
                    recipe.researchPrerequisite = null;
                }
            }

            List<FactionDef> factions = DefDatabase<FactionDef>.AllDefsListForReading
                .Where(d => d.modContentPack.PackageId == ModContentPack.CoreModPackageId).ToList();
            factions.AddRange(DefDatabase<FactionDef>.AllDefsListForReading
                .Where(d => d.modContentPack.PackageId == ModContentPack.RoyaltyModPackageId).ToList());

            foreach (var faction in factions) 
            {
                if (faction != FactionDefOf.PlayerColony && faction != FactionDefOf.PlayerTribe)
                {
                    if (faction == FactionDefOf.Empire 
                        || faction == FactionDef.Named("OutlanderCivil")
                        || faction == FactionDef.Named("TribeCivil")
                        || faction == FactionDef.Named("OutlanderRough")
                        || faction == FactionDef.Named("Pirate"))
                    {
                        faction.hidden = true;
                    }
                    faction.settlementGenerationWeight = 0f;
                    faction.goodwillDailyFall = 0;
                    faction.goodwillDailyGain = 0;
                    faction.startingGoodwill = new IntRange(80, 80);
                    faction.permanentEnemy = false;
                    faction.mustStartOneEnemy = false;
                }
            }
        }

        [HarmonyPatch(typeof(BiomeDef), "AllWildAnimals", MethodType.Getter)]
        public static class AnimalRemoval
        {
            [HarmonyPostfix]
            public static void Postfix(ref IEnumerable<PawnKindDef> __result)
            {
                __result = __result.ToList().Where(x => x.race?.modContentPack?.PackageId != ModContentPack.CoreModPackageId
                && x.race?.modContentPack?.PackageId != ModContentPack.CoreModPackageId);
            }
        }
    }
}

