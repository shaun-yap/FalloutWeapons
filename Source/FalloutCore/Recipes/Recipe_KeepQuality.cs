using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FalloutCore
{
    public class Recipe_KeepQuality : RecipeWorker
    {

    }

    [HarmonyPatch(typeof(GenRecipe))]
    [HarmonyPatch("MakeRecipeProducts")]
    public static class Patch_MakeRecipeProducts
    {
        public static void Postfix(ref IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
        {
            if (recipeDef.workerClass == typeof(Recipe_KeepQuality))
            {
                foreach (var i in ingredients)
                {
                    if (i.TryGetQuality(out QualityCategory qc))
                    {
                        foreach (var t in __result)
                        {
                            var comp = t.TryGetComp<CompQuality>();
                            if (comp != null)
                            {
                                comp.SetQuality(qc, ArtGenerationContext.Colony);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}

