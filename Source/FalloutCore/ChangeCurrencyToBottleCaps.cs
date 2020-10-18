using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace FalloutCore
{
    [StaticConstructorOnStartup]
    public static class ChangeCurrencyToBottleCaps
    {
        static ChangeCurrencyToBottleCaps()
        {
            var replacedSilver = ThingDef.Named("ReplacedSilver");
            replacedSilver.label = ThingDefOf.Silver.label;
            replacedSilver.description = ThingDefOf.Silver.description;

            var dummyBottleCaps = ThingDef.Named("DummyBottleCaps");
            ThingDefOf.Silver.label = dummyBottleCaps.label;
            ThingDefOf.Silver.description = dummyBottleCaps.description;

            List<ThingDef> things = DefDatabase<ThingDef>.AllDefsListForReading;
            foreach (ThingDef thing in things)
            {
                var silverValue = thing?.costList?.Where(x => x.thingDef == ThingDefOf.Silver)?.FirstOrDefault();
                if (silverValue != null)
                {
                    Log.Message("Replace " + thing + " with replaced silver", true);
                    ThingDefCountClass newValue = new ThingDefCountClass();
                    newValue.thingDef = replacedSilver;
                    newValue.count = silverValue.count;
                    thing.costList.Add(newValue);
                    thing.costList.Remove(silverValue);
                }
            }

            List<TerrainDef> terrains = DefDatabase<TerrainDef>.AllDefsListForReading;
            foreach (TerrainDef terrain in terrains)
            {
                var silverValue = terrain?.costList?.Where(x => x.thingDef == ThingDefOf.Silver)?.FirstOrDefault();
                if (silverValue != null)
                {
                    Log.Message("Replace " + terrain + " with replaced silver", true);
                    ThingDefCountClass newValue = new ThingDefCountClass();
                    newValue.thingDef = ThingDef.Named("ReplacedSilver");
                    newValue.count = silverValue.count;
                    terrain.costList.Add(newValue);
                    terrain.costList.Remove(silverValue);
                }
            }

            List<RecipeDef> recipes = DefDatabase<RecipeDef>.AllDefsListForReading;
            foreach (RecipeDef recipe in recipes)
            {
                foreach (var ingredient in recipe?.ingredients)
                {
                    var silverValue = ingredient.filter.AllowedThingDefs?.Where(x => x == ThingDefOf.Silver)?.FirstOrDefault();
                    if (silverValue != null)
                    {
                        Log.Message("Replace " + recipe + " with replaced silver", true);
                        ingredient.filter.AllowedThingDefs.ToList().Add(ThingDef.Named("ReplacedSilver"));
                        ingredient.filter.AllowedThingDefs.ToList().Remove(ThingDefOf.Silver);
                    }
                }
            }

            ThingDefOf.Silver.stuffProps.categories.Remove(StuffCategoryDefOf.Metallic);
            var dummy = DefDatabase<StuffCategoryDef>.GetNamed("DummyMetallic", true);
            ThingDefOf.Silver.stuffProps.categories.Add(dummy);
        }
    }
}

